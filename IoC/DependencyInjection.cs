using ApplicationServices.Services.Security;
using Domains;
using Domains.Services.MessageBroker;
using Domains.Services.Security;
using MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using Repository.Pagination;

namespace IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Security
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            // MessageBroker
            services.AddSingleton<IHistoryMessageBrokerPublisher, HistoryMessageBrokerPublisher>();

            // Repository
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
            services.AddScoped<IPaginationRepository, PaginationRepository>();

            return services;
        }

        public static void BindConfigurations(this IConfiguration configuration)
        {
            AppSettings.Authentication = configuration.GetSection("Authentication").Get<Authentication>();
            AppSettings.Broker = configuration.GetSection("MessageBroker").Get<BrokerConfiguration>();           
        }
    }
}