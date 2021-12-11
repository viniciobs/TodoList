using Repository.DTOs.Tasks;
using Repository.Interfaces.Base;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface ITaskRepository : IRepository
	{
		public Task<AssignTaskResult> Assign(AssignTaskData data);
	}
}