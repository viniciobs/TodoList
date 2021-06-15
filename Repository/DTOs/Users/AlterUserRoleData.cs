using Domains;
using System;

namespace Repository.DTOs.Users
{
	public class AlterUserRoleData
	{
		public Guid AuthenticatedUser { get; set; }

		public Guid TargetUser { get; set; }

		public UserRole NewRole { get; set; }
	}
}