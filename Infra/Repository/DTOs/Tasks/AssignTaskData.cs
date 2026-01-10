using Domains;
using System;
using System.Text.Json.Serialization;

namespace Repository.DTOs.Tasks
{
	public record AssignTaskData
	{
		public string Description { get; init; }

		[JsonIgnore]
		public User TargetUser { get; init; } 
		public Guid TargetUserId => TargetUser.Id;

		[JsonIgnore]
		public User CreatorUser { get; init; }
		public Guid CreatorUserId => CreatorUser.Id;
	}
}