using System;

namespace Repository.DTOs.Users
{
	public class UserResult
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}