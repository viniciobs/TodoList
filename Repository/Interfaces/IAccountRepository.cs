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

		public Task ChangePassword(Guid userId, ChangePasswordData data);

		public Task Delete(Guid userId);
	}
}