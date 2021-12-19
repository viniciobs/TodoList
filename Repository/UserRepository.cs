using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Users;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
	public class UserRepository : _Commom.Repository, IUserRepository
	{
		private readonly IPaginationRepository _pagination;

		public UserRepository(ApplicationContext context, IPaginationRepository paginationRepository)
			: base(context)
		{
			_pagination = paginationRepository;
		}

		public async Task<PaginationResult<UserResult>> GetAsync(UserFilter filter)
		{
			var query = _db.User.AsNoTracking().Filter(filter);
			var users = await query.OrderBy(x => x.Name).Paginate(filter).ToArrayAsync();
			var total = await query.CountAsync();

			return _pagination.Paginate(
				filter,
				total,
				users.Select(user => UserResult.Convert(user))
			); 
		}

		public async Task<User> FindAsync(Guid id, bool? isActive = null)
		{
			var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && (isActive == null || x.IsActive == isActive.Value));
			if (user == null) throw new NotFoundException(typeof(User));

			return user;
		}

		public async Task AlterUserRoleAsync(AlterUserRoleData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));

			if (!Enum.IsDefined(typeof(UserRole), data.NewRole)) throw new MissingArgumentsException("New role is invalid");

			var authenticatedUser = await _db.User.FindAsync(data.AuthenticatedUser);
			if (authenticatedUser == null) throw new NotFoundException(typeof(User), "Authenticated user not found");
			if (authenticatedUser.Role != UserRole.Admin) throw new PermissionException("The authenticated user has no rights to alter user's role");

			var targetUser = await _db.User.FindAsync(data.TargetUser);
			if (targetUser == null) throw new NotFoundException(typeof(User), "Target user not found");

			authenticatedUser.AlterUserRole(targetUser, data.NewRole);
		}				

		public async Task<bool> ExistsAllAsync(Guid[] usersIds)
		{
			var resultQty = await _db.User.AsNoTracking().Where(x => usersIds.Contains(x.Id)).Select(x => x.Id).CountAsync();

			return resultQty == usersIds.Length;
		}
	}
}