using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportGenerator.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ReportGeneratorService>();
        services.AddHostedService<SchedulerService>();
    }).Build();

await host.RunAsync();