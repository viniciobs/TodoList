using System;

namespace Repository.DTOs.Users
{
	public class ChangePasswordData
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
}