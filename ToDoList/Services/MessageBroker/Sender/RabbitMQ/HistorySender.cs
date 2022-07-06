using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoList.API.Configurations.ServicesConfigurations;
using ToDoList.API.Services.MessageBroker.Sender.Models;

namespace ToDoList.API.Services.MessageBroker.Sender.RabbitMQ
{
    public class HistorySender : IHistoryMessageBroker
    {
        private readonly Broker _broker;
        private readonly ConnectionFactory _factory;

        public HistorySender(IConfiguration configuration)
        {
            var brokerConfigurationSection = configuration.GetSection("MessageBroker");

            _broker = brokerConfigurationSection.Get<Broker>();
            _factory = new ConnectionFactory() { HostName = _broker.HostName };
        }

        public async Task PostHistoryAsync(HistoryData history)
        {
            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(history));

            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.BasicPublish(exchange: _broker.Exchange, routingKey: _broker.RoutingKey, body: message);
                }
            }
        }
    }
}