using Domains;
using Repository.DTOs.Accounts;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IAccountRepository : IRepository
	{
		public Task<AuthenticationResult> Authenticate(AuthenticationData data);
		public Task Create(CreateAccountData data);
		public void ChangePassword(User user, ChangePasswordData data);
		public Task Delete(Guid userId);
		public Task AlterStatus(Guid userId, bool active);
		public Task Edit(User user, EditData data);
	}
}