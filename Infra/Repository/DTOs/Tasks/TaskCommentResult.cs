using System;
using static Domains.User.Task;

namespace Repository.DTOs.Tasks
{
	public class TaskCommentResult
	{
		public DateTime CreatedAt { get; set; }
		public Guid TaskId { get; set; }
		public string Comment { get; set; }
		public Guid UserId { get; set; }

		public static TaskCommentResult Convert(TaskComment taskComment)
		{
			return new TaskCommentResult()
			{
				TaskId = taskComment.TaskId,
				Comment = taskComment.Text,
				CreatedAt = taskComment.CreatedAt,
				UserId = taskComment.CreatedByUserId
			};
		}
	}
}
