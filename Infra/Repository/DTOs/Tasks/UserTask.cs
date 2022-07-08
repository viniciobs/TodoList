using Domains;
using System;

namespace Repository.DTOs.Tasks
{
	public class UserTask
	{
		public User User { get; set; }
		public Guid TaskId { get; set; }
	}
}
