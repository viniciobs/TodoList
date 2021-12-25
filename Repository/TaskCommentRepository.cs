using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
using Repository.Interfaces._Commom;
using Repository.Interfaces_Commom;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
	public class TaskCommentRepository : _Commom.Repository, ITaskCommentRepository, IFilterRepository<TaskCommentResult, Domains.User.Task.TaskComment, TaskCommentFilter>
	{
		private readonly IPaginationRepository _pagination;

		public TaskCommentRepository(ApplicationContext context, IPaginationRepository pagination)
			: base(context)
		{
			_pagination = pagination;
		}

		public async Task<TaskCommentResult> AddCommentAsync(TaskCommentData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (data.User == null) throw new MissingArgumentsException(nameof(data.User));
			if (data.TaskId == null) throw new MissingArgumentsException(nameof(data.TaskId));
			if (string.IsNullOrEmpty(data.Comment.Trim())) throw new MissingArgumentsException(nameof(data.Comment));

			var task = await _db.Task.FindAsync(data.TaskId);
			if (task == null) throw new NotFoundException("Couldn't find task");

			var comment = data.User.AddComment(task, data.Comment);

			await _db.TaskComment.AddAsync(comment);

			_db.Entry(comment.CreatedBy).State = EntityState.Unchanged;
			_db.Entry(comment.Task).State = EntityState.Unchanged;

			return TaskCommentResult.Convert(comment);
		}		

		public async Task<PaginationResult<TaskCommentResult>> GetAsync(TaskCommentFilter filter)
		{
			return await _pagination.Paginate(this, filter);
		}

		public IQueryable<User.Task.TaskComment> ApplyFilter(IQueryable<User.Task.TaskComment> source, TaskCommentFilter filter)
		{
			if (filter == null) return source;

			bool filterByTask = filter.TaskId != default;
			bool filterByPeriod = filter.CreatedBetween.HasValue;

			Period period = filter.CreatedBetween;

			if (filterByPeriod)
				source = source.Where(x => period.IsBetween(x.CreatedAt));

			if (filterByTask)
				source = source.Where(x => x.TaskId == filter.TaskId);

			return source;
		}

		public IQueryable<User.Task.TaskComment> ApplyIncludes(IQueryable<User.Task.TaskComment> source)
		{
			return source;
		}

		public IQueryable<TaskCommentResult> CastToDTO(IQueryable<User.Task.TaskComment> source)
		{
			return source.Select(comment => TaskCommentResult.Convert(comment));
		}

		public IQueryable<User.Task.TaskComment> OrderBy(IQueryable<User.Task.TaskComment> source)
		{
			return source.OrderBy(comment => comment.CreatedAt);
		}
	}
}
