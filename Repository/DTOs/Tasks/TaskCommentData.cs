using Domains;
using System;
using static Domains.User;

namespace Repository.DTOs.Tasks
{
	public class TaskCommentData
	{
		public User User { get; set; }
		public Guid TaskId { get; set; }
		public string Comment { get; set; }
	}
}
