using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IHistoryRepository
    {
        Task AddHistoryAsync(string serializedData);
    }
}