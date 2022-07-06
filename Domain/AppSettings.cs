using Domains.Services.MessageBroker;

namespace Domains
{
    public static class AppSettings
    {
        public static BrokerConfiguration Broker { get; set; } = new BrokerConfiguration();
    }
}