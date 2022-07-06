using ApplicationServices.Services.MessageBroker;
using ApplicationServices.Services.Security;
using Domains;
using Domains.Services.MessageBroker;
using Domains.Services.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Security
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            // MessageBroker
            services.AddSingleton<IHistoryMessageBrokerProducer, HistoryMessageBrokerProducer>();

            return services;
        }

        public static void BindConfigurations(this IConfiguration configuration)
        {
            var bindings = new Dictionary<string, object>
            {
                { "MessageBroker", AppSettings.Broker },
                { "Authentication", AppSettings.Authentication }
            };

            foreach (var item in bindings)
            {
                configuration.GetSection(item.Key).Bind(item.Value);
            }
        }
    }
}