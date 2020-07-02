using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using SME.SR.Workers.SGP.Controllers;
using System;
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
                                        IModel channel,
                                        IConnection connection,
                                        IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<RabbitBackgroundListener>();
            _scopeFactory = scopeFactory;
            _connection = connection;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _channel = channel;
            InitRabbit();
        }

        private void InitRabbit()
        {
            _channel.ExchangeDeclare(RotasRabbit.ExchangeListenerWorkerRelatorios, ExchangeType.Topic);
            _channel.QueueDeclare(RotasRabbit.FilaWorkerRelatorios, false, false, false, null);

            //esperando todas as filas a partir de 'sme.sr.workers.sgp'
            _channel.QueueBind(RotasRabbit.FilaWorkerRelatorios, RotasRabbit.ExchangeListenerWorkerRelatorios, "*", null);
            _channel.BasicQos(0, 1, false);
        }

        private async Task HandleMessage(string content)
        {
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
                                return;
                            }
                        }

                        _logger.LogInformation($"[ INFO ] Method not found to action: {request.Action}");
                    }

                }
                catch (NegocioException ex)
                {
                    SentrySdk.CaptureException(ex);
                }
                catch (Exception ex)
                {
                    NotificarUsuarioRelatorioComErro(request);
                    SentrySdk.CaptureException(ex);
                }
            }
        }

        private void NotificarUsuarioRelatorioComErro(FiltroRelatorioDto request)
        {
            var mensagemRabbit = new MensagemRabbit(string.Empty, null, request.CodigoCorrelacao, request.UsuarioLogadoRF);
            var mensagem = JsonConvert.SerializeObject(mensagemRabbit);
            var body = Encoding.UTF8.GetBytes(mensagem);

            _channel.QueueBind(RotasRabbit.FilaSgp, RotasRabbit.ExchangeSgp, RotasRabbit.RotaRelatorioComErro);
            _channel.BasicPublish(RotasRabbit.ExchangeSgp, RotasRabbit.RotaRelatorioComErro, null, body);
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
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
                await HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            WorkerAttribute worker = GetWorkerAttribute(typeof(WorkerSGPController));
            _channel.BasicConsume(worker.WorkerQueue, false, consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}
