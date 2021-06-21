using Repository.DTOs.Users;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IUserRepository : IRepository
	{
		public Task<UserResult[]> Get(string name, string login);

		public Task<UserResult> Get(Guid id);

		public Task AlterUserRole(AlterUserRoleData data);
	}
}