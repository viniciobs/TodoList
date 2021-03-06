using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Users;
using Repository.Interfaces;
using Repository.Interfaces._Commom;
using Repository.Interfaces_Commom;
using Repository.Pagination;
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : Base.Repository, IUserRepository, IFilterRepository<UserResult, User, UserFilter>
    {
        private readonly IPaginationRepository _pagination;

        public UserRepository(ApplicationContext context, IPaginationRepository paginationRepository)
            : base(context)
        {
            _pagination = paginationRepository;
        }

        public async Task<PaginationResult<UserResult>> GetAsync(UserFilter filter)
        {
            return await _pagination.Paginate<UserResult, User, UserFilter>(this, filter);
        }

        public async Task<User> FindAsync(Guid id, bool? isActive = null)
        {
            if (id == null || id == default) throw new MissingArgumentsException(nameof(id));

            var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && (isActive == null || x.IsActive == isActive.Value));
            if (user == null) throw new NotFoundException(typeof(User));

            return user;
        }

        public async Task AlterUserRoleAsync(AlterUserRoleData data)
        {
            if (data == null) throw new MissingArgumentsException(nameof(data));
            if (data.AuthenticatedUser == null || data.AuthenticatedUser == default) throw new MissingArgumentsException(nameof(data.AuthenticatedUser));
            if (data.TargetUser == null || data.TargetUser == default) throw new MissingArgumentsException(nameof(data.TargetUser));

            if (!Enum.IsDefined(typeof(UserRole), data.NewRole)) throw new MissingArgumentsException("New role is invalid");

            var authenticatedUser = await _db.User.FindAsync(data.AuthenticatedUser);
            if (authenticatedUser == null) throw new NotFoundException(typeof(User), "Authenticated user not found");
            if (authenticatedUser.Role != UserRole.Admin) throw new PermissionException("The authenticated user has no rights to alter user's role");

            var targetUser = await _db.User.FindAsync(data.TargetUser);
            if (targetUser == null) throw new NotFoundException(typeof(User), "Target user not found");

            authenticatedUser.AlterUserRole(targetUser, data.NewRole);
        }

        public IQueryable<User> ApplyFilter(IQueryable<User> source, UserFilter filter)
        {
            if (filter == null) return source;

            bool filterByName = !string.IsNullOrEmpty(filter.Name);
            bool filterByLogin = !string.IsNullOrEmpty(filter.Login);
            bool filterByStatus = filter.IsActive.HasValue;

            if (filterByName)
                source = source.Where((x) => EF.Functions.Like(x.Name, filter.Name.Like()));

            if (filterByLogin)
                source = source.Where((x) => EF.Functions.Like(x.Login, filter.Login.Like()));

            if (filterByStatus)
                source = source.Where((x) => x.IsActive == (bool)filter.IsActive);

            return source;
        }

        public IQueryable<User> OrderBy(IQueryable<User> source)
        {
            return source.OrderBy(user => user.Name);
        }

        public IQueryable<UserResult> CastToDTO(IQueryable<User> source)
        {
            return source.Select(x => UserResult.Convert(x));
        }

        public IQueryable<User> ApplyIncludes(IQueryable<User> source)
        {
            return source;
        }
    }
}