using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository.Interfaces;
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
        private readonly IHistoryRepository _service;

        public HistoryConsumerService(IOptions<BrokerConfiguration> configuration, IHistoryRepository service)
        {
            _configuration = configuration.Value;
            _service = service;

            var factory = new ConnectionFactory { HostName = _configuration.Host };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                try
                {
                    Console.WriteLine($"DeliveryTag: {eventArgs.DeliveryTag}");

                    var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    await _service.AddHistoryAsync(content);

                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{exception.Message} at {DateTime.Now}");

                    _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(_configuration.Queue, autoAck: false, consumer);
        }
    }
}