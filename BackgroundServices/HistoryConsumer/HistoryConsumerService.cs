using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    internal class HistoryConsumerService : BackgroundService
    {
        private readonly BrokerConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public HistoryConsumerService(IOptions<BrokerConfiguration> configuration)
        {
            _configuration = configuration.Value;

            var factory = new ConnectionFactory { HostName = _configuration.Host };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                try
                {
                    var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    // TODO: Save history

                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"DeliveryTag: {eventArgs.DeliveryTag}");
                    Console.WriteLine($"{exception.Message} at {DateTime.Now}");

                    _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(_configuration.Queue, autoAck: false, consumer);

            return Task.CompletedTask;
        }
    }
}