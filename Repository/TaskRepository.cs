using DataAccess;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
	public class TaskRepository : _Commom.Repository, ITaskRepository
	{
		private readonly IPaginationRepository _pagination;

		public TaskRepository(ApplicationContext context, IPaginationRepository pagination) 
			: base(context)
		{
			_pagination = pagination;
		}	

		public async Task<TaskResult> Assign(AssignTaskData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.Description)) throw new MissingArgumentsException(nameof(data.Description));
			if (data.CreatorUser == null) throw new PermissionException("It's necessary to be authenticated");
			if (data.TargetUser == null) throw new NotFoundException("Target user was not found");
			if (!data.CreatorUser.IsActive) throw new RuleException("To assign task, the user must be active");
			if (!data.TargetUser.IsActive) throw new RuleException("Tasks can be assigned to active users only");
			
			var task = data.CreatorUser.AssignTask(data.TargetUser, data.Description);

			await _db.Task.AddAsync(task);

			_db.Entry(data.CreatorUser).State = EntityState.Unchanged;
			_db.Entry(data.TargetUser).State = EntityState.Unchanged;

			return TaskResult.Convert(task);
		}

		public async Task<TaskResult> Find(Guid userId, Guid id)
		{
			var task = await _db.Task.AsNoTracking().Include(x => x.CreatorUser).Include(x => x.TargetUser).SingleOrDefaultAsync(x => x.Id == id && (x.TargetUserId == userId || x.CreatorUserId == userId));
			if (task == null) throw new NotFoundException(typeof(Domains.User.Task));

			return TaskResult.Convert(task); 
		}

		public async Task Finish(UserTask data)
		{
			var task = await _db.Task.SingleOrDefaultAsync(x => x.Id == data.TaskId);
			if (task == null) throw new NotFoundException(typeof(Domains.User.Task));

			data.User.FinishTask(task);					
		}

		public async Task<PaginationResult<TaskResult>> Get(TaskFilter filter)
		{
			var query = _db.Task.AsNoTracking().Include(x => x.CreatorUser).Include(x => x.TargetUser).Filter(filter);
			var tasks = await query.OrderBy(x => x.CreatedAt).Paginate(filter).ToArrayAsync();
			var total = await query.CountAsync();

			return _pagination.Paginate(
				filter,
				total,
				tasks.Select(tasks => TaskResult.Convert(tasks))
			);
		}

		public async Task Reopen(UserTask data)
		{
			var task = await _db.Task.SingleOrDefaultAsync(x => x.Id == data.TaskId);
			if (task == null) throw new NotFoundException(typeof(Domains.User.Task));

			data.User.ReopenTask(task);
		}
	}	
}
