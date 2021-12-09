using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Users;
using Repository.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IUserRepository : IRepository
	{
		public Task<PaginationResult<UserResult>> Get(UserFilter filter);
		public Task<UserResult> Find(Guid id, bool? isActive = null);
		public Task AlterUserRole(AlterUserRoleData data);
	}
}