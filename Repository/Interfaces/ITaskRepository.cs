using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface ITaskRepository : IRepository
	{
		public Task<TaskResult> Assign(AssignTaskData data);
		public Task<TaskResult> Find(Guid userId, Guid id);
		public Task<PaginationResult<TaskResult>> Get(TaskFilter data);
		public Task Finish(UserTask data);
		public Task Reopen(UserTask data);
	}
}