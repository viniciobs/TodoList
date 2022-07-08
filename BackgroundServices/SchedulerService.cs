using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    internal abstract class SchedulerService : IHostedService
    {
        private Timer _timer;

        protected abstract int StartHour { get; }
        protected abstract int IntervalInHours { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var action = new Action(() =>
            {
                var delayBeforeStart = GetIntervalToNextRun();
                var delay = Task.Delay(delayBeforeStart);
                delay.Wait();

                _timer = new Timer(Run, null, TimeSpan.Zero, TimeSpan.FromHours(IntervalInHours));
            });

            Task.Run(action);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private TimeSpan GetIntervalToNextRun()
        {
            var nextRunTime = DateTime.Today.AddHours(StartHour);
            var firstInterval = nextRunTime.Subtract(DateTime.Now);

            if (firstInterval >= TimeSpan.Zero) return firstInterval;

            nextRunTime = nextRunTime.AddDays(1);
            return nextRunTime.Subtract(DateTime.Now);
        }

        protected abstract void Run(object state);
    }
}