using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Services
{
    public class RabbitBackgroundListener : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public RabbitBackgroundListener(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<RabbitBackgroundListener>();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            // TODO Separação das variáveis de ambiente e adição da connection no DI
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("sme.sr.workers", ExchangeType.Topic);
            _channel.QueueDeclare("sme.sr.workers.sgp", false, false, false, null);
            _channel.QueueBind("sme.sr.workers.sgp", "sme.sr.workers", "sme.sr.workers.sgp.*", null);
            _channel.BasicQos(0, 1, false);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

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

            _channel.BasicConsume("demo.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // TODO Parse do command e reflection para criação do comando correto
            _logger.LogInformation($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
