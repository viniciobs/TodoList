using Domains.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TaskComment = Domains.User.Task.TaskComment;

namespace Domains
{
	public partial class User
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public string Login { get; private set; }
		public string Password { get; private set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; private set; }
		public UserRole Role { get; private set; }

		[NotMapped]
		public virtual ICollection<Task> TargetTasks { get; private set; }

		[NotMapped]
		public virtual ICollection<Task> CreatedTasks { get; private set; }

		[NotMapped]
		public ICollection<TaskComment> TaskComments { get; private set; }

		private User()
		{
			CreatedAt = DateTime.UtcNow;

			TargetTasks = new HashSet<Task>();
			CreatedTasks = new HashSet<Task>();
			TaskComments = new HashSet<TaskComment>();
		}

		public static User New(string name, string login)
		{
			if (string.IsNullOrEmpty(name)) throw new MissingArgumentsException(nameof(name));
			if (string.IsNullOrEmpty(login)) throw new MissingArgumentsException(nameof(login));

			var user = new User()
			{
				Id = Guid.NewGuid(),
				Name = name,
				Login = login,
				Role = UserRole.Normal
			};

			return user;
		}

#if DEBUG		

		/// <summary>
		/// Generate a user instance with admin role.
		/// Must only be used for tests purposes.
		/// </summary>
		/// <returns>Admins user.</returns>
		public static User NewAdmin()
		{
			var admin = new User()
			{
				Id = Guid.NewGuid(),
				Name = "Administrator",
				Login = "admin",
				Role = UserRole.Admin
			};

			return admin;
		}

#endif

		public void SetPassword(string password)
		{
			if (string.IsNullOrEmpty(password)) throw new MissingArgumentsException(nameof(password));

			Password = password;
		}

		public void Activate()
		{
			IsActive = true;
		}

		public void Deactivate()
		{
			if (TargetTasks.Any(x => x.CompletedAt == null)) throw new RuleException("Cannot deactivate account while user still has pending tasks");

			IsActive = false;
		}

		public void AlterUserRole(User targetUser, UserRole role)
		{
			if (Role != UserRole.Admin) throw new PermissionException("No rigths to alter roles");
			if (targetUser == null) throw new MissingArgumentsException(nameof(targetUser));
			if (this == targetUser) throw new RuleException("Users can't alter its own role");
			if (role == targetUser.Role) throw new RuleException("Given user has this role already");

			targetUser.Role = role;
		}

		public Task AssignTask(User targetUser, string taskDescription)
		{
			var task = Task.New(this, targetUser, taskDescription);

			CreatedTasks.Add(task);
			targetUser.TargetTasks.Add(task);

			return task;
		}		

		public void FinishTask(Task taskToFinish)
		{
			if (taskToFinish == null) throw new MissingArgumentsException(nameof(taskToFinish));

			var userCanFinish = taskToFinish.CreatorUserId == Id || taskToFinish.TargetUserId == Id;
			if (!userCanFinish) throw new PermissionException("User has no permission to finish this task");

			taskToFinish.Finish();
		}

		public void ReopenTask(Task taskToReopen)
		{
			if (taskToReopen == null) throw new MissingArgumentsException(nameof(taskToReopen));

			var userCanReopen = taskToReopen.CreatorUserId == Id || taskToReopen.TargetUserId == Id;
			if (!userCanReopen) throw new PermissionException("User has no permission to reopen this task");

			taskToReopen.Reopen();
		}

		public TaskComment AddComment(Task task, string comment)
		{
			if (task == null) throw new MissingArgumentsException(nameof(task));

			var userCanComment = task.CreatorUserId == Id || task.TargetUserId == Id;
			if (!userCanComment) throw new PermissionException("User has no permission to comment this task");

			return task.AddComment(this, comment);
		}		
	}
}