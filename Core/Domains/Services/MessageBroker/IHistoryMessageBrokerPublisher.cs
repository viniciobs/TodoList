using System.Threading.Tasks;

namespace Domains.Services.MessageBroker
{
    public interface IHistoryMessageBrokerPublisher
    {
        Task PostHistoryAsync(HistoryData message);
    }
}