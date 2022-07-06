using Microsoft.Extensions.Hosting;

namespace ReportGenerator.Services
{
    internal class SchedulerService : IHostedService
    {
        private const int intervalInHours = 24;
        private readonly ReportGeneratorService _service;
        private Timer _timer;

        public SchedulerService(ReportGeneratorService service)
        {
            _service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var action = new Action(() =>
            {
                var delayBeforeStart = GetIntervalToNextRun();
                var delay = Task.Delay(delayBeforeStart);
                delay.Wait();

                _timer = new Timer(GenerateReport, null, TimeSpan.Zero, TimeSpan.FromHours(intervalInHours));
            });

            Task.Run(action);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private TimeSpan GetIntervalToNextRun()
        {
            var nextRunTime = DateTime.Today.AddHours(12);
            var firstInterval = nextRunTime.Subtract(DateTime.Now);

            if (firstInterval >= TimeSpan.Zero) return firstInterval;

            nextRunTime = nextRunTime.AddDays(1);
            return nextRunTime.Subtract(DateTime.Now);
        }

        private void GenerateReport(object state)
        {
            _service.Generate();
        }
    }
}