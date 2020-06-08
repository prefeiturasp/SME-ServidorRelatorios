using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using SME.SR.Workers.SGP.Controllers;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Services
{
    // TODO Turn this into a generic listener or common lib
    public class RabbitBackgroundListener : IHostedService
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection _connection;

        private IModel _channel;

        // TODO move to a dynamic listener factory (Attributes???)
        public static string ExchangeName = "sme.sr.workers";

        public static string QueueName = "sme.sr.workers.sgp";

        public static string RouteKeys = "sme.sr.workers.sgp.*";

        public RabbitBackgroundListener(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<RabbitBackgroundListener>();
            _scopeFactory = scopeFactory;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            // TODO Separação das variáveis de ambiente e adição da connection no DI
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "user", Password = "bitnami" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
            _channel.QueueDeclare(QueueName, false, false, false, null);
            _channel.QueueBind(QueueName, ExchangeName, RouteKeys, null);
            _channel.BasicQos(0, 1, false);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void HandleMessage(string content)
        {
            _logger.LogInformation($"[ INFO ] Messaged received: {content}");

            var request = JsonConvert.DeserializeObject<FiltroRelatorioDto>(content);
            //string actionName = (string)request["action"];
            MethodInfo[] methods = typeof(WorkerSGPController).GetMethods();

            foreach (MethodInfo method in methods)
            {
                ActionAttribute actionAttribute = GetActionAttribute(method);
                if (actionAttribute != null && actionAttribute.Name == request.Action)
                {
                    // TODO Turn into a injected controller or dynamically registered
                    _logger.LogInformation($"[ INFO ] Invoking action: {request.Action}");

                    var serviceProvider = _scopeFactory.CreateScope().ServiceProvider;
                    var controller = (WorkerSGPController)serviceProvider.GetRequiredService<WorkerSGPController>();
                    //var mediator = (IMediator)serviceProvider.GetRequiredService<IMediator>();

                    method.Invoke(controller, new object[] { request });

                    _logger.LogInformation($"[ INFO ] Action terminated: {request.Action}");
                    return;
                }
            }

            _logger.LogInformation($"[ INFO ] Method not found to action: {request.Action}");
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
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.Span;
                var content = System.Text.Encoding.UTF8.GetString(body);
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            WorkerAttribute worker = GetWorkerAttribute(typeof(WorkerSGPController));
            _channel.BasicConsume(worker.WorkerQueue, false, consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}
