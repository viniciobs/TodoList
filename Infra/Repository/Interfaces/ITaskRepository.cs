using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface ITaskRepository : IRepository
	{
		public Task<TaskResult> AssignAsync(AssignTaskData data);
		public Task<TaskResult> FindAsync(Guid userId, Guid id);
		public Task<PaginationResult<TaskResult>> GetAsync(TaskFilter data);
		public Task FinishAsync(UserTask data);
		public Task ReopenAsync(UserTask data);		
	}
}