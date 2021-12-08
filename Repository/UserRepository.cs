using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs;
using Repository.DTOs.Users;
using Repository.Interfaces;
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
	public class UserRepository : Repository, IUserRepository
	{
		#region Constructor

		public UserRepository(ApplicationContext context)
			: base(context)
		{ }

		#endregion Constructor

		#region Methods

		public async Task<UserResult[]> Get(UserFilter filter)
		{
			var users = await _db.User.AsNoTracking().Filter(filter).OrderBy(x => x.Name).Paginate((PaginationFilter)filter).ToArrayAsync();

			return users.Select(x => new UserResult()
			{
				Id = x.Id,
				Name = x.Name,
				Login = x.Login,
				Role = x.Role,
				CreatedAt = x.CreatedAt
			}).ToArray();
		}

		public async Task<UserResult> Find(Guid id, bool? isActive = null)
		{
			var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && (isActive == null || x.IsActive == isActive.Value));
			if (user == null) throw new NotFoundException(typeof(User));

			return new UserResult()
			{
				Id = user.Id,
				Login = user.Login,
				Name = user.Name,
				Role = user.Role,
				CreatedAt = user.CreatedAt
			};
		}

		public async Task AlterUserRole(AlterUserRoleData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));

			var authenticatedUser = await _db.User.FindAsync(data.AuthenticatedUser);
			if (authenticatedUser == null) throw new NotFoundException(typeof(User), "Authenticated user not found");
			if (authenticatedUser.Role != UserRole.Admin) throw new PermissionException("The authenticated user has no rights to alter user's role.");

			var targetUser = await _db.User.FindAsync(data.TargetUser);
			if (targetUser == null) throw new NotFoundException(typeof(User), "Target user not found");

			authenticatedUser.AlterUserRole(targetUser, data.NewRole);
		}				

		#endregion Methods
	}
}