using Domains;
using Repository.DTOs.Users;
using Repository.Interfaces.Base;
using Repository.Pagination;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUserRepository : IRepository
    {
        public Task<PaginationResult<UserResult>> GetAsync(UserFilter filter);

        public Task<User> FindAsync(Guid id, bool? isActive = null);

        public Task AlterUserRoleAsync(AlterUserRoleData data);
    }
}