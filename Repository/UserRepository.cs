using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Users;
using Repository.Interfaces;
using Repository.Util;
using System;
using System.Linq;
using System.Linq.Expressions;
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

		public async Task<UserResult[]> Get(string name, string login)
		{
			var filterPredicate = GetFilterExpression(name, login);

			var users = await _db.User.AsNoTracking().Where(filterPredicate).OrderBy(x => x.Name).ToArrayAsync();

			return users.Select(x => new UserResult()
			{
				Id = x.Id,
				Name = x.Name,
				Login = x.Login,
				CreatedAt = x.CreatedAt
			}).ToArray();
		}

		public async Task<UserResult> Get(Guid id)
		{
			var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
			if (user == null) throw new NotFoundException(typeof(User));

			return new UserResult()
			{
				Id = user.Id,
				Login = user.Login,
				Name = user.Name,
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

		private Expression<Func<User, bool>> GetFilterExpression(string name, string login)
		{
			Expression<Func<User, bool>> predicate;

			var filterByName = !string.IsNullOrEmpty(name);
			var filterByLogin = !string.IsNullOrEmpty(login);

			var hasFilter = filterByName || filterByLogin;

			if (!hasFilter) return (x) => true;

			var filterBoth = filterByName && filterByLogin;

			if (filterBoth)
			{
				predicate = (x) => EF.Functions.Like(x.Name, name.Like()) && EF.Functions.Like(x.Login, login.Like());
			}
			else if (filterByName)
			{
				predicate = (x) => EF.Functions.Like(x.Name, name.Like());
			}
			else
			{
				predicate = (x) => EF.Functions.Like(x.Login, login.Like());
			}

			return predicate;
		}

		#endregion Methods
	}
}