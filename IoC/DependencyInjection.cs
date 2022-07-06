using ApplicationServices.Services.MessageBroker;
using Domains.Services.MessageBroker;
using IoC.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IHistoryMessageBrokerProducer, HistoryMessageBrokerProducer>();

            return services;
        }

        public static void BindConfigurations(this IConfiguration configuration)
        {
            configuration.BindBrokerSettings();
        }
    }
}