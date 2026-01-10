using Domains;
using Domains.Services.MessageBroker;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageBroker
{
    public class HistoryMessageBrokerPublisher : IHistoryMessageBrokerPublisher
    {
        private readonly ConnectionFactory _factory;

        public HistoryMessageBrokerPublisher()
        {
            _factory = new ConnectionFactory() 
            { 
                HostName = AppSettings.Broker.HostName,
                Port = AppSettings.Broker.Port,
                UserName = AppSettings.Broker.UserName,
                Password = AppSettings.Broker.Password                
            };
        }

        public async Task PostHistoryAsync(HistoryData history)
        {
            var serializerOptions = new JsonSerializerOptions { AllowTrailingCommas = false, WriteIndented = false };
            var serialized = JsonSerializer.Serialize(history, serializerOptions);
            var message = Encoding.UTF8.GetBytes(serialized);

            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.BasicPublishAsync(exchange: AppSettings.Broker.Exchange,
                routingKey: AppSettings.Broker.RoutingKey,
                body: message);
        }
    }
}