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

		public static UserResult Convert(User user)
		{
			return new UserResult()
			{
				Id = user.Id,
				Login = user.Login,
				Name = user.Name,
				Role = user.Role,
				CreatedAt = user.CreatedAt,
				IsActive = user.IsActive
			};
		}
	}
}