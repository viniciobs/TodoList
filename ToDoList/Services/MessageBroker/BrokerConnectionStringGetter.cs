using Microsoft.Extensions.Configuration;

namespace ToDoList.API.Services.MessageBroker
{
    public static class BrokerConnectionStringGetter
    {
        public static string GetBrokerConnection(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("MessageBroker");
        }
    }
}