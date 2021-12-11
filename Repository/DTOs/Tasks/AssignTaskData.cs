using Domains;
using System;

namespace Repository.DTOs.Tasks
{
	public class AssignTaskData
	{
		public string Description { get; set; }
		public User TargetUser { get; set; } 
		public User CreatorUser { get; set; }
	}
}