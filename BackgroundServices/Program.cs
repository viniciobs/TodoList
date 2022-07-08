﻿using BackgroundServices.ReportGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackgroundServices
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services =>
                 {
                     services.AddSingleton<ReportGeneratorService>();
                     services.AddHostedService<SchedulerService>();
                 }).Build();

            host.Run();
        }
    }
}