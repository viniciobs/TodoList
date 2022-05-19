using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoList.API.Services.MessageBroker.Sender.Models;

namespace ToDoList.API.Services.MessageBroker.Sender
{
    public class HistorySender : IHistoryMessageBroker
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public HistorySender(IConfiguration configuration)
        {
            string connectionString = configuration.GetBrokerConnection();
            if (string.IsNullOrEmpty(connectionString?.Trim())) throw new ArgumentNullException("Configure your connection string for Microsoft Azure ServiceBus");

            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender("history");
        }

        public async Task PostHistoryAsync(HistoryData history)
        {
            var payload = JsonSerializer.Serialize(history);
            var message = new ServiceBusMessage(payload);

            await _sender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}