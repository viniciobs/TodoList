using Domains;
using System;

namespace Repository.DTOs.Accounts
{
	public class AuthenticationResult
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public string Token { get; set; }

		public UserRole Role { get; set; }
	}
}