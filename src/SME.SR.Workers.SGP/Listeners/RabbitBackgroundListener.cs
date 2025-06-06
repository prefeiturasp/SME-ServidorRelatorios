﻿using Elastic.Apm;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
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
        private IConnection conexaoRabbit;
        private IModel canalRabbit;
        private IConnectionFactory factory;

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IServicoTelemetria servicoTelemetria;
        private readonly TelemetriaOptions telemetriaOptions;
        private readonly IMediator mediator;
        private readonly ConfiguracaoFilasRabbitOptions configuracaoFilasRabbit;
        public RabbitBackgroundListener(IServiceScopeFactory serviceScopeFactory,
                                        IServicoTelemetria servicoTelemetria,
                                        TelemetriaOptions telemetriaOptions,
                                        IConfiguration configuration,
                                        IMediator mediator,
                                        ConfiguracaoFilasRabbitOptions configuracaoFilasRabbit)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
            this.telemetriaOptions = telemetriaOptions ?? throw new ArgumentNullException(nameof(telemetriaOptions));            
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.configuracaoFilasRabbit = configuracaoFilasRabbit ?? throw new ArgumentNullException(nameof(configuracaoFilasRabbit));

             if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value,
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            ConectarRabbitMQ();
        }

        private void ConectarRabbitMQ()
        {
            conexaoRabbit = factory.CreateConnection();
            canalRabbit = conexaoRabbit.CreateModel();
            canalRabbit.BasicQos(0, 1, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.WorkerRelatorios, ExchangeType.Direct, true, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.WorkerRelatoriosDeadletter, ExchangeType.Direct, true, false);
            DeclararFilas();
        }

        private void DeclararFilas()
        {
            DeclararFilasConfiguradas(ExchangeRabbit.WorkerRelatorios);
            DeclararFilasPorRota(typeof(RotasRabbitSR), ExchangeRabbit.WorkerRelatorios);
        }

        private void DeclararFilasPorRota(Type tipoRotas, string exchange)
        {
            if (configuracaoFilasRabbit.GetFilas.Any())
                return;
            foreach (var fila in tipoRotas.ObterConstantesPublicas<string>()
                                          .Where(r => !configuracaoFilasRabbit.GetFilasIgnoradas.Contains(r)))
            {
                canalRabbit.QueueDeclare(fila, true, false, false, ArgumentosRabbitSR.ObterConfiguracao(fila));
                canalRabbit.QueueBind(fila, exchange, fila, null);
            }
        }

        private void DeclararFilasConfiguradas(string exchange)
        {
            foreach (var fila in configuracaoFilasRabbit.GetFilas)
            {
                canalRabbit.QueueDeclare(fila, true, false, false, ArgumentosRabbitSR.ObterConfiguracao(fila));
                canalRabbit.QueueBind(fila, exchange, fila, null);
            }
        }

        private async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.Span);
            var rota = ea.RoutingKey;

            var filtroRelatorio = JsonConvert.DeserializeObject<FiltroRelatorioDto>(content);

            var transacao = telemetriaOptions.Apm ? Agent.Tracer.StartTransaction(rota, "WorkerRabbitSR") : null;

            try
            {
                if (!content.Equals("null"))
                {
                    MethodInfo[] methodsWorkerSgp = typeof(WorkerSGPController).GetMethods();

                    foreach (MethodInfo method in methodsWorkerSgp)
                    {
                        ActionAttribute actionAttribute = GetActionAttribute(method);

                        if (actionAttribute != null && actionAttribute.Name == filtroRelatorio.Action)
                        {
                            using var scope = serviceScopeFactory.CreateScope();
                            var useCase = scope.ServiceProvider.GetService(actionAttribute.TipoCasoDeUso);                            

                            //-> registrando use case
                            if (actionAttribute.TipoCasoDeUso.GetInterfaces().Contains(typeof(IUseCase)))
                            {
                                var metodoExecutar = actionAttribute.TipoCasoDeUso
                                    .GetInterface("IUseCase")
                                    .GetMethod("Executar");

                                if (metodoExecutar != null)
                                {
                                    await servicoTelemetria.RegistrarAsync(async () =>
                                        await (Task)metodoExecutar.Invoke(useCase, new object[] { filtroRelatorio }),
                                        "RabbitMQ_SR",
                                        filtroRelatorio.Action,
                                        rota,
                                        filtroRelatorio.Mensagem.ToString());
                                }
                            }
                            else
                            {
                                var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
                                var controller = serviceProvider.GetRequiredService<WorkerSGPController>();
                                await method.InvokeAsync(controller, new object[] { filtroRelatorio, useCase });
                            }

                            canalRabbit.BasicAck(ea.DeliveryTag, false);
                            return;
                        }
                    }

                    string info = $"[ INFO ] Method not found to action: {filtroRelatorio.Action}";
                    throw new NegocioException(info);
                }
                else
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
            }
            catch (NegocioException nex)
            {
                canalRabbit.BasicAck(ea.DeliveryTag, false);

                NotificarUsuarioRelatorioComErro(filtroRelatorio, nex.Message);

                await RegistrarLogErro(ea.RoutingKey, filtroRelatorio, nex, LogNivel.Negocio);

                transacao?.CaptureException(nex);
            }
            catch (Exception ex)
            {
                canalRabbit.BasicReject(ea.DeliveryTag, false);

                NotificarUsuarioRelatorioComErro(filtroRelatorio, "Não foi possível gerar o relatório solicitado. Solicite novamente mais tarde.");

                await RegistrarLogErro(ea.RoutingKey, filtroRelatorio, ex, LogNivel.Critico);

                transacao?.CaptureException(ex);
            }
            finally
            {
                transacao?.End();
            }
        }
        private async Task RegistrarLogErro(string rota, FiltroRelatorioDto mensagemRabbit, Exception ex, LogNivel nivel)
        {
            var mensagem = $"{mensagemRabbit.UsuarioLogadoRF} - {mensagemRabbit.CodigoCorrelacao.ToString()[..3]} - ERRO - {rota}";
            await mediator.Send(new SalvarLogViaRabbitCommand(mensagem, nivel, ex.Message));
        }

        private async Task RegistrarLogErro(string mensagem, LogNivel logNivel, string observacao = "")
        {
            await mediator.Send(new SalvarLogViaRabbitCommand(mensagem, logNivel, observacao));
        }

        private async Task RegistrarLogErro(string erro)
        {
            await mediator.Send(new SalvarLogViaRabbitCommand("Erro de Tratamento da Mensagem ao Processar", LogNivel.Critico, erro));
        }

        private void NotificarUsuarioRelatorioComErro(FiltroRelatorioDto request, string erro)
        {
            var mensagemRabbit = new MensagemRabbit(string.Empty, new RetornoWorkerDto(erro), request.CodigoCorrelacao, request.UsuarioLogadoRF);
            var mensagem = JsonConvert.SerializeObject(mensagemRabbit);
            var body = Encoding.UTF8.GetBytes(mensagem);

            canalRabbit.BasicPublish(ExchangeRabbit.Sgp, request.RotaErro, null, body);
        }

        private ActionAttribute GetActionAttribute(MethodInfo method)
        {
            ActionAttribute actionAttribute = (ActionAttribute)method.GetCustomAttribute(typeof(ActionAttribute));
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
            RegistrarConsumer();
            return Task.CompletedTask;
        }

        private void RegistrarConsumer()
        {
            var consumer = new EventingBasicConsumer(canalRabbit);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await TratarMensagem(ea);
                }
                catch (AlreadyClosedException ex)
                {
                    if (canalRabbit.IsClosed)
                        ReconectarRabbitMQ();
                    await RegistrarLogErro($"Erro Conexão RabbitMQ Fechada - {ea.RoutingKey}", LogNivel.Critico, $"{ex.Message} - {Encoding.UTF8.GetString(ea.Body.Span)}");
                }
                catch (Exception ex)
                {
                    await RegistrarLogErro(ex.Message);
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            RegistrarConsumer(consumer);
        }

        private void ReconectarRabbitMQ()
        {
            ConectarRabbitMQ();
            RegistrarConsumer();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canalRabbit.Close();
            conexaoRabbit.Close();
            return Task.CompletedTask;
        }

        private void RegistrarConsumer(EventingBasicConsumer consumer)
        {
            foreach (var fila in configuracaoFilasRabbit.GetFilas)
                canalRabbit.BasicConsume(fila, false, consumer); 
            if (!configuracaoFilasRabbit.GetFilas.Any())
                foreach (var fila in typeof(RotasRabbitSR).ObterConstantesPublicas<string>()
                                                          .Where(r => !configuracaoFilasRabbit.GetFilasIgnoradas.Contains(r)))
                    canalRabbit.BasicConsume(fila, false, consumer);
        }
    }
}
