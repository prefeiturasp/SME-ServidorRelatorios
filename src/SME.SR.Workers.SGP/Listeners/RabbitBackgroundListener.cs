using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IConnection _connection;
        private readonly IConfiguration configuration;
        private readonly IModel _channel;

        public RabbitBackgroundListener(ILoggerFactory loggerFactory,
                                        IServiceScopeFactory scopeFactory,
                                        IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<RabbitBackgroundListener>();
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            InitRabbit();
        }

        private void InitRabbit()
        {

            _channel.ExchangeDeclare(ExchangeRabbit.ExchangeListenerWorkerRelatorios, ExchangeType.Direct, true, false);
            _channel.ExchangeDeclare(ExchangeRabbit.ExchangeListenerWorkerRelatoriosDeadletter, ExchangeType.Direct, true, false);

            DeclararFilasPorRota(typeof(RotasRabbit), "solicitados", ExchangeRabbit.ExchangeListenerWorkerRelatorios, ExchangeRabbit.ExchangeListenerWorkerRelatoriosDeadletter);
            DeclararFilasPorRota(typeof(RotasRabbit), "processando", ExchangeRabbit.ExchangeListenerWorkerRelatorios, ExchangeRabbit.ExchangeListenerWorkerRelatoriosDeadletter);
            DeclararFilasPorRota(typeof(RotasRabbit), "prontos", ExchangeRabbit.ExchangeSgp, ExchangeRabbit.ExchangeSgpDeadLetter);
            DeclararFilasPorRota(typeof(RotasRabbit), "erro", ExchangeRabbit.ExchangeSgp, ExchangeRabbit.ExchangeSgpDeadLetter);
            DeclararFilasPorRota(typeof(RotasRabbit), "correlacao", ExchangeRabbit.ExchangeSgp, ExchangeRabbit.ExchangeSgpDeadLetter);

            _channel.BasicQos(0, 1, false);
        }


        private void DeclararFilasPorRota(Type tipoRotas, string busca, string exchange, string exchangeDeadletter)
        {
            foreach (var fila in tipoRotas.ObterConstantesPublicas<string>().Where(a => a.Contains(busca)))
            {
                var args = new Dictionary<string, object>()
                    {
                        { "x-dead-letter-exchange", exchange}
                    };
                _channel.QueueDeclare(fila, true, false, false, args);
                _channel.QueueBind(fila, exchange, fila, null);

                var filaDeadLetter = $"{fila}.deadletter";
                _channel.QueueDeclare(filaDeadLetter, true, false, false, null);
                _channel.QueueBind(filaDeadLetter, exchangeDeadletter, filaDeadLetter, null);
            }
        }

        private async Task HandleMessage(BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.Span);
            using (SentrySdk.Init(configuration.GetSection("Sentry:DSN").Value))
            {
                var request = JsonConvert.DeserializeObject<FiltroRelatorioDto>(content);
                try
                {
                    _logger.LogInformation($"[ INFO ] Messaged received: {content}");

                    if (!content.Equals("null"))
                    {
                        MethodInfo[] methods = typeof(WorkerSGPController).GetMethods();

                        foreach (MethodInfo method in methods)
                        {
                            ActionAttribute actionAttribute = GetActionAttribute(method);
                            if (actionAttribute != null && actionAttribute.Name == request.Action)
                            {
                                _logger.LogInformation($"[ INFO ] Invoking action: {request.Action}");

                                var serviceProvider = _scopeFactory.CreateScope().ServiceProvider;

                                var controller = serviceProvider.GetRequiredService<WorkerSGPController>();
                                var useCase = serviceProvider.GetRequiredService(actionAttribute.TipoCasoDeUso);

                                await method.InvokeAsync(controller, new object[] { request, useCase });

                                _logger.LogInformation($"[ INFO ] Action terminated: {request.Action}");
                                _channel.BasicAck(ea.DeliveryTag, false);
                                return;
                            }
                        }

                        string info = $"[ INFO ] Method not found to action: {request.Action}";
                        _logger.LogInformation(info);
                        throw new NegocioException(info);
                    }
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (NegocioException ex)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    NotificarUsuarioRelatorioComErro(request, ex.Message);
                    SentrySdk.AddBreadcrumb($"Erros: {ex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                }
                catch (Exception ex)
                {
                    _channel.BasicReject(ea.DeliveryTag, false);
                    SentrySdk.AddBreadcrumb($"Erros: {ex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    NotificarUsuarioRelatorioComErro(request, "Ocorreu um erro interno, por favor tente novamente");
                }
            }
        }

        private void NotificarUsuarioRelatorioComErro(FiltroRelatorioDto request, string erro)
        {
            var mensagemRabbit = new MensagemRabbit(string.Empty, new RetornoWorkerDto(erro), request.CodigoCorrelacao, request.UsuarioLogadoRF);
            var mensagem = JsonConvert.SerializeObject(mensagemRabbit);
            var body = Encoding.UTF8.GetBytes(mensagem);

            _channel.BasicPublish(ExchangeRabbit.ExchangeSgp, request.RotaErro, null, body);
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await HandleMessage(ea);
                }
                catch(Exception ex)
                {
                    SentrySdk.AddBreadcrumb($"Erro ao tratar mensagem {ea.DeliveryTag}", "erro", null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    _channel.BasicReject(ea.DeliveryTag, false);
                }                
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            RegistrarConsumer(consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }

        private void RegistrarConsumer(EventingBasicConsumer consumer)
        {
            foreach (var fila in typeof(RotasRabbit).ObterConstantesPublicas<string>())
                _channel.BasicConsume(fila, false, consumer);
        }
    }
}
