using Repository.DTOs.History;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IHistoryRepository
	{
		Task AddHistory(AddHistoryData data); 
	}
}
