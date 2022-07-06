using Domains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC.Dependencies
{
    internal static class BrokerDependencyResolver
    {
        public static void BindBrokerSettings(this IConfiguration configuration)
        {
            configuration.GetSection("MessageBroker").Bind(AppSettings.Broker);
        }
    }
}