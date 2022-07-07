using Domains;
using Domains.Services.MessageBroker;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationServices.Services.MessageBroker
{
    public class HistoryMessageBrokerProducer : IHistoryMessageBrokerProducer
    {
        private readonly ConnectionFactory _factory;

        public HistoryMessageBrokerProducer()
        {
            _factory = new ConnectionFactory() { HostName = AppSettings.Broker.HostName };
        }

        public async Task PostHistoryAsync(HistoryData history)
        {
            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(history));

            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.BasicPublish(exchange: AppSettings.Broker.Exchange,
                routingKey: AppSettings.Broker.RoutingKey,
                body: message);
        }
    }
}