using Repository.DTOs.Users;
using System;

namespace Repository.DTOs.Tasks
{
	public class AssignTaskResult
	{
		public Guid Id { get; set; }
		public string Description { get;  set; }
		public UserResult TargetUser { get; set; }
	}
}
