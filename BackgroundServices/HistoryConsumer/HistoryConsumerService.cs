using Microsoft.EntityFrameworkCore.Metadata;
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
    internal class HistoryConsumerService(
        IOptions<BrokerConfiguration> configuration,
        IHistoryRepository service) 
        : BackgroundService
    {
        private readonly IHistoryRepository _service = service;
        private readonly BrokerConfiguration _configuration = configuration.Value;
        private IConnection _connection;
        private IChannel _channel;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
             var factory = new ConnectionFactory { HostName = _configuration.Host };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(options: null, cancellationToken);

            await base.StartAsync(cancellationToken);
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {            
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                try
                {
                    Console.WriteLine($"DeliveryTag: {eventArgs.DeliveryTag}");

                    var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    await _service.AddHistoryAsync(content);

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{exception.Message} at {DateTime.Now}");

                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(_configuration.Queue, autoAck: false, consumer);
        }
    }
}