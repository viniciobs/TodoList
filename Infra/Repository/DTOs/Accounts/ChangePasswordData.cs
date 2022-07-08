using Domains;

namespace Repository.DTOs.Accounts
{
	public class ChangePasswordData
	{		
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
}