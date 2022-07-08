namespace BackgroundServices.ReportGenerator
{
    internal class ReportGeneratorService : SchedulerService
    {
        protected override int StartHour { get => 20; }
        protected override int IntervalInHours { get => 24; }

        protected override void Run(object state)
        {
            throw new System.NotImplementedException();
        }
    }
}