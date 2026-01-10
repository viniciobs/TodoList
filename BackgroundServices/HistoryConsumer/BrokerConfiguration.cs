namespace BackgroundServices
{
    internal record BrokerConfiguration
    {
        public string Host { get; init; }
        public string Queue { get; init; }
    }
}