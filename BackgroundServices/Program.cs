using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Repository.Interfaces;
using System;
using Domains.Services.MessageBroker;

namespace BackgroundServices
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("ToDoListDB");                        

            using IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services =>
                 {
                     services.Configure<BrokerConfiguration>(configuration.GetSection("MessageBroker"));

                     services.AddSingleton<IHistoryRepository>(x => ActivatorUtilities.CreateInstance<HistoryRepository>(x, connectionString));

                    //  services.AddHostedService<ReportGeneratorService>();
                     services.AddHostedService<HistoryConsumerService>();
                 }).Build();

            host.Run();
        }
    }
}