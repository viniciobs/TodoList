using DataAccess;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Tasks;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace Repository
{
	public class TaskRepository : _Commom.Repository, ITaskRepository
	{
		public TaskRepository(ApplicationContext context) 
			: base(context)
		{
		}	

		public async Task<AssignTaskResult> Assign(AssignTaskData data)
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

			return new AssignTaskResult()
			{
				Id = task.Id,
				Description = task.Description,
				TargetUser = UserResult.Convert(data.TargetUser)
			};
		}		
	}
}
