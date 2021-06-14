using Repository.DTOs.Users;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IUserRepository : IRepository
	{
		public Task<AuthenticationResult> Authenticate(AuthenticationData data);

		public Task<CreateUserResult> Create(CreateUserData data);

		public Task<UserResult[]> Get(string name, string login);

		public Task<UserResult> Get(Guid id);

		public Task ChangePassword(Guid userId, ChangePasswordData data);

		public Task Delete(Guid userId);
	}
}