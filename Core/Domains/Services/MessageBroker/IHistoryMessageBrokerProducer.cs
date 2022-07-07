using System.Threading.Tasks;

namespace Domains.Services.MessageBroker
{
    public interface IHistoryMessageBrokerProducer
    {
        Task PostHistoryAsync(HistoryData message);
    }
}