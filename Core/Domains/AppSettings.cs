using Domains.Services.MessageBroker;
using Domains.Services.Security;

namespace Domains
{
    public static class AppSettings
    {
        public static BrokerConfiguration Broker { get; set; } = new BrokerConfiguration();
        public static Authentication Authentication { get; set; } = new Authentication();
    }
}