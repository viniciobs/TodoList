using Domains.Exceptions;
using System;
using Task = Domains.User.Task;

namespace Domains
{
	public partial class User
	{
		public partial class Task
		{
			public class TaskComment
			{
				public Guid Id { get; private set; }
				public string Text { get; private set; }
				public Guid TaskId { get; private set; }
				public Task Task { get; private set; }
				public Guid CreatedByUserId { get; private set; }
				public User CreatedBy { get; private set; }
				public DateTime CreatedAt { get; private set; }

				public TaskComment()
				{ }

				private TaskComment(Task task, User creator, string text)
				{
					if (task == null) throw new MissingArgumentsException(nameof(task));
					if (creator == null) throw new MissingArgumentsException(nameof(creator));
					if (string.IsNullOrEmpty(text)) throw new MissingArgumentsException(nameof(text));

					CreatedBy = creator;
					CreatedByUserId = creator.Id;
					Task = task;
					TaskId = task.Id;
					Text = text;
					CreatedAt = DateTime.UtcNow;
				}

				internal static TaskComment New(Task task, User creator, string text)
				{
					return new TaskComment(task, creator, text);
				}
			}
		}
	}
}