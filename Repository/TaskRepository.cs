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
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
	public class TaskRepository : _Commom.Repository, ITaskRepository, IFilterRepository<TaskResult, User.Task, TaskFilter>
	{
		private readonly IPaginationRepository _pagination;

		public TaskRepository(ApplicationContext context, IPaginationRepository pagination) 
			: base(context)
		{
			_pagination = pagination;
		}	

		public async Task<TaskResult> AssignAsync(AssignTaskData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.Description)) throw new MissingArgumentsException(nameof(data.Description));
			if (data.CreatorUser == null) throw new MissingArgumentsException(nameof(data.CreatorUser));
			if (data.TargetUser == null) throw new MissingArgumentsException(nameof(data.TargetUser));
			if (!data.CreatorUser.IsActive) throw new RuleException("To assign task, the user must be active");
			if (!data.TargetUser.IsActive) throw new RuleException("Tasks can be assigned to active users only");
			
			var task = data.CreatorUser.AssignTask(data.TargetUser, data.Description);

			await _db.Task.AddAsync(task);

			_db.Entry(data.CreatorUser).State = EntityState.Detached;
			_db.Entry(data.TargetUser).State = EntityState.Detached;

			return TaskResult.Convert(task);
		}	

		public async Task<TaskResult> FindAsync(Guid userId, Guid id)
		{
			var task = await _db.Task.AsNoTracking().Include(x => x.CreatorUser).Include(x => x.TargetUser).SingleOrDefaultAsync(x => x.Id == id && (x.TargetUserId == userId || x.CreatorUserId == userId));
			if (task == null) throw new NotFoundException(typeof(Domains.User.Task));

			return TaskResult.Convert(task); 
		}

		public async Task FinishAsync(UserTask data)
		{
			var task = await _db.Task.SingleOrDefaultAsync(x => x.Id == data.TaskId);
			if (task == null) throw new NotFoundException(typeof(Domains.User.Task));

			data.User.FinishTask(task);					
		}

		public async Task<PaginationResult<TaskResult>> GetAsync(TaskFilter filter)
		{
			return await _pagination.Paginate(this, filter);
		}
		
		public async Task ReopenAsync(UserTask data)
		{
			var task = await _db.Task.SingleOrDefaultAsync(x => x.Id == data.TaskId);
			if (task == null) throw new NotFoundException(typeof(User.Task));

			data.User.ReopenTask(task);
		}

		public IQueryable<User.Task> ApplyFilter(IQueryable<User.Task> source, TaskFilter filter)
		{
			if (filter == null) return source;

			bool filterByStatus = filter.Completed.HasValue;
			bool filterByCompletedPeriod = filter.CompletedBetween.HasValue;
			bool filterByCreatorUser = filter.CreatorUser.HasValue;
			bool filterByTargetUser = filter.TargetUser.HasValue;

			Period period = filter.CompletedBetween;

			if (filterByStatus)
				source = source.Where((x) => x.CompletedAt.HasValue == (bool)filter.Completed);

			if (filterByCompletedPeriod)
				source = source.Where(x => x.CompletedAt.HasValue && period.IsBetween(x.CompletedAt.Value));

			if (filterByCreatorUser && filterByTargetUser && filter.UserFilter == FilterHelper.OR)
			{
				source = source.Where(x => x.CreatorUserId == filter.CreatorUser || x.TargetUserId == filter.TargetUser);
			}
			else
			{
				if (filterByCreatorUser)
					source = source.Where(x => x.CreatorUserId == filter.CreatorUser);

				if (filterByTargetUser)
					source = source.Where(x => x.TargetUserId == filter.TargetUser);
			}

			return source;
		}

		public IQueryable<User.Task> ApplyIncludes(IQueryable<User.Task> source)
		{
			return source.Include(x => x.CreatorUser).Include(x => x.TargetUser);
		}

		public IQueryable<TaskResult> CastToDTO(IQueryable<User.Task> source)
		{
			return source.Select(task => TaskResult.Convert(task));
		}

		public IQueryable<User.Task> OrderBy(IQueryable<User.Task> source)
		{
			return source.OrderBy(x => x.CreatedAt);
		}
	}	
}
