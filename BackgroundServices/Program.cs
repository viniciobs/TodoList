using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BackgroundServices
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            using IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services =>
                 {
                     services.Configure<BrokerConfiguration>(configuration.GetSection("MessageBroker"));

                     services.AddHostedService<ReportGeneratorService>();
                     services.AddHostedService<HistoryConsumerService>();
                 }).Build();

            host.Run();
        }
    }
}