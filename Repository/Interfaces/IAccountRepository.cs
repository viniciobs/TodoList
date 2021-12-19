using Domains;
using Repository.DTOs.Accounts;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IAccountRepository : IRepository
	{
		public Task<AuthenticationResult> AuthenticateAsync(AuthenticationData data);
		public Task CreateAsync(CreateAccountData data);
		public Task DeleteAsync(Guid userId);
		public Task AlterStatusAsync(Guid userId, bool active);
		public Task EditAsync(User user, EditData data);
		public void ChangePassword(User user, ChangePasswordData data);
	}
}