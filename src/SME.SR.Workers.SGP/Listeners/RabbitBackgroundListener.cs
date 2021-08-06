using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;
using Sentry.Protocol;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using SME.SR.Workers.SGP.Commons.Attributes;
using SME.SR.Workers.SGP.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Services
{
    // TODO Turn this into a generic listener or common lib
    public class RabbitBackgroundListener : IHostedService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly string sentryDSN;
        private readonly IConnection conexaoRabbit;
        private readonly IConfiguration configuration;
        private readonly IModel canalRabbit;

        public RabbitBackgroundListener(IServiceScopeFactory serviceScopeFactory,
                                        IConfiguration configuration)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory)); ;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.sentryDSN = configuration.GetValue<string>("Sentry:DSN");            

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value
            };

            conexaoRabbit = factory.CreateConnection();
            canalRabbit = conexaoRabbit.CreateModel();

            canalRabbit.BasicQos(0, 1, false);

            canalRabbit.ExchangeDeclare(ExchangeRabbit.WorkerRelatorios, ExchangeType.Direct, true, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.WorkerRelatoriosDeadletter, ExchangeType.Direct, true, false);

            DeclararFilas();
        }

        private void DeclararFilas()
        {
            DeclararFilasPorRota(typeof(RotasRabbit), "solicitados", ExchangeRabbit.WorkerRelatorios, ExchangeRabbit.WorkerRelatoriosDeadletter);
            DeclararFilasPorRota(typeof(RotasRabbit), "processando", ExchangeRabbit.WorkerRelatorios, ExchangeRabbit.WorkerRelatoriosDeadletter);
            DeclararFilasPorRota(typeof(RotasRabbit), "prontos", ExchangeRabbit.Sgp, ExchangeRabbit.SgpDeadLetter);
            DeclararFilasPorRota(typeof(RotasRabbit), "erro", ExchangeRabbit.Sgp, ExchangeRabbit.SgpDeadLetter);
            DeclararFilasPorRota(typeof(RotasRabbit), "correlacao", ExchangeRabbit.Sgp, ExchangeRabbit.SgpDeadLetter);
        }


        private void DeclararFilasPorRota(Type tipoRotas, string busca, string exchange, string exchangeDeadletter)
        {
            foreach (var fila in tipoRotas.ObterConstantesPublicas<string>().Where(a => a.Contains(busca)))
            {
                var args = new Dictionary<string, object>()
                    {
                        { "x-dead-letter-exchange", exchangeDeadletter}
                    };
                canalRabbit.QueueDeclare(fila, true, false, false, args);
                canalRabbit.QueueBind(fila, exchange, fila, null);

                var filaDeadLetter = $"{fila}.deadletter";
                canalRabbit.QueueDeclare(filaDeadLetter, true, false, false, null);
                canalRabbit.QueueBind(filaDeadLetter, exchangeDeadletter, fila, null);
            }
        }

        private async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.Span);

            using (SentrySdk.Init(sentryDSN))
            {
                var mensagemRabbit = JsonConvert.DeserializeObject<FiltroRelatorioDto>(content);
                try
                {
                    if (!content.Equals("null"))
                    {
                        MethodInfo[] methods = typeof(WorkerSGPController).GetMethods();

                        foreach (MethodInfo method in methods)
                        {
                            ActionAttribute actionAttribute = GetActionAttribute(method);
                            if (actionAttribute != null && actionAttribute.Name == mensagemRabbit.Action)
                            {
                                var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

                                var controller = serviceProvider.GetRequiredService<WorkerSGPController>();
                                var useCase = serviceProvider.GetRequiredService(actionAttribute.TipoCasoDeUso);

                                await method.InvokeAsync(controller, new object[] { mensagemRabbit, useCase });

                                canalRabbit.BasicAck(ea.DeliveryTag, false);
                                return;
                            }
                        }
                        string info = $"[ INFO ] Method not found to action: {mensagemRabbit.Action}";
                        throw new NegocioException(info);
                    }
                    else
                        canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
                catch (NegocioException nex)
                {
                    canalRabbit.BasicAck(ea.DeliveryTag, false);
                    SentrySdk.AddBreadcrumb($"Erros: {nex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(nex);
                    RegistrarSentry(ea, mensagemRabbit, nex);
                    NotificarUsuarioRelatorioComErro(mensagemRabbit, nex.Message);
                }
                catch (Exception ex)
                {
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                    SentrySdk.AddBreadcrumb($"Erros: {ex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    RegistrarSentry(ea, mensagemRabbit, ex);
                }
            }
        }

        private void RegistrarSentry(BasicDeliverEventArgs ea, FiltroRelatorioDto mensagemRabbit, Exception ex)
        {
            SentrySdk.CaptureMessage($"{mensagemRabbit.UsuarioLogadoRF} - {mensagemRabbit.CodigoCorrelacao.ToString().Substring(0, 3)} - ERRO - {ea.RoutingKey}", SentryLevel.Error);
            SentrySdk.CaptureException(ex);
        }

        private void NotificarUsuarioRelatorioComErro(FiltroRelatorioDto request, string erro)
        {
            var mensagemRabbit = new MensagemRabbit(string.Empty, new RetornoWorkerDto(erro), request.CodigoCorrelacao, request.UsuarioLogadoRF);
            var mensagem = JsonConvert.SerializeObject(mensagemRabbit);
            var body = Encoding.UTF8.GetBytes(mensagem);

            canalRabbit.BasicPublish(ExchangeRabbit.Sgp, request.RotaErro, null, body);
        }

        private WorkerAttribute GetWorkerAttribute(Type type)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs)
            {
                if (attr is WorkerAttribute)
                {
                    return (WorkerAttribute)attr;
                }
            }

            return null;
        }

        private ActionAttribute GetActionAttribute(MethodInfo method)
        {
            ActionAttribute actionAttribute = (ActionAttribute)
                method.GetCustomAttribute(typeof(ActionAttribute));
            return actionAttribute;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(canalRabbit);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await TratarMensagem(ea);
                }
                catch (Exception ex)
                {
                    SentrySdk.AddBreadcrumb($"Erro ao tratar mensagem {ea.DeliveryTag}", "erro", null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            RegistrarConsumer(consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canalRabbit.Close();
            conexaoRabbit.Close();
            return Task.CompletedTask;
        }

        private void RegistrarConsumer(EventingBasicConsumer consumer)
        {
            foreach (var fila in typeof(RotasRabbit).ObterConstantesPublicas<string>())
                canalRabbit.BasicConsume(fila, false, consumer);
        }
    }
}
