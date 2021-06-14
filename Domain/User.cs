using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using TaskComment = Domains.User.Task.TaskComment;

namespace Domains
{
	public partial class User
	{
		#region Properties

		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public string Login { get; private set; }
		public string Password { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public UserRole Role { get; private set; }

		[NotMapped]
		public virtual ICollection<Task> TargetTasks { get; private set; }

		[NotMapped]
		public virtual ICollection<Task> CreatedTasks { get; private set; }

		[NotMapped]
		public ICollection<TaskComment> TaskComments { get; private set; }

		#endregion Properties

		#region Constructor

		private User()
		{
			CreatedAt = DateTime.UtcNow;

			TargetTasks = new HashSet<Task>();
			CreatedTasks = new HashSet<Task>();
			TaskComments = new HashSet<TaskComment>();
		}

		#endregion Constructor

		#region Methods

		public static User New(string name, string login)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

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
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

			Password = password;
		}

		public Task SetTask(User targetUser, string taskDescription)
		{
			var task = Task.New(this, targetUser, taskDescription);

			CreatedTasks.Add(task);
			targetUser.TargetTasks.Add(task);

			return task;
		}

		public void FinishTask(Task taskToFinish)
		{
			if (taskToFinish == null) throw new ArgumentNullException(nameof(taskToFinish));

			var userCanFinish = taskToFinish.CreatorUser == this || taskToFinish.TargetUser == this;
			if (!userCanFinish) throw new SecurityException("User has no permission to finish this task");

			taskToFinish.Finish();
		}

		public void ReopenTask(Task taskToReopen)
		{
			if (taskToReopen == null) throw new ArgumentNullException(nameof(taskToReopen));

			var userCanReopen = taskToReopen.CreatorUser == this || taskToReopen.TargetUser == this;
			if (!userCanReopen) throw new SecurityException("User has no permission to reopen this task");

			taskToReopen.Reopen();
		}

		public void AddComment(Task task, string comment)
		{
			if (task == null) throw new ArgumentNullException(nameof(task));

			var userCanComment = task.CreatorUser == this || task.TargetUser == this;
			if (!userCanComment) throw new SecurityException("User has no permission to comment this task");

			task.AddComment(this, comment);
		}

		public void AlterUserRole(User targetUser, UserRole role)
		{
			if (this.Role != UserRole.Admin) throw new InvalidOperationException("No rigths to alter roles");
			if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
			if (this == targetUser) throw new InvalidOperationException("Users can't alter its own role");
			if (role == targetUser.Role) throw new ApplicationException("Given user has this role already");

			targetUser.Role = role;
		}

		#endregion Methods
	}
}