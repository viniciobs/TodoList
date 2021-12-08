using Domains;
using System;

namespace Repository.DTOs.Users
{
	public class UserResult
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		public DateTime CreatedAt { get; set; }
		public UserRole Role { get; set; }
		public bool IsActive { get; set; }
	}
}