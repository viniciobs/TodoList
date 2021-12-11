using Repository.DTOs.Users;
using System;

namespace Repository.DTOs.Tasks
{
	public class TaskResult
	{
		public Guid Id { get; set; }
		public string Description { get;  set; }
		public UserResult TargetUser { get; set; }
		public UserResult CreatorUser { get; set; }

		public static TaskResult Convert(Domains.User.Task task)
		{
			return new TaskResult()
			{
				Id = task.Id,
				Description = task.Description,
				TargetUser = UserResult.Convert(task.TargetUser),
				CreatorUser = UserResult.Convert(task.CreatorUser)
			};
		}
	}
}
