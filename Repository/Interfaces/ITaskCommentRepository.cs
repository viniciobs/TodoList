using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces.Base;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface ITaskCommentRepository : IRepository
	{
		public Task<TaskCommentResult> AddCommentAsync(TaskCommentData data);
		public Task<PaginationResult<TaskCommentResult>> GetAsync(TaskCommentFilter data);
	}
}
