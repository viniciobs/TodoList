using System.Threading.Tasks;
using ToDoList.API.Services.MessageBroker.Sender.Models;

namespace ToDoList.API.Services.MessageBroker.Sender
{
    public interface IHistoryMessageBroker
    {
        Task PostHistoryAsync(HistoryData message);
    }
}