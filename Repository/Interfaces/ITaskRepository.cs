using Repository.DTOs.Tasks;
using Repository.Interfaces.Base;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	internal interface ITaskRepository : IRepository
	{
		public Task AssignTask(AssingTaskData data);
	}
}