namespace Domains.Services.MessageBroker
{
    public class BrokerConfiguration
    {
        public string HostName { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}