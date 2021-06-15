using DataAccess;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System;
using Domains;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Repository.Util;

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

		public async Task<CreateUserResult> Create(CreateUserData data)
		{
			var loginExists = await _db.User.AnyAsync(x => x.Login == data.Login);
			if (loginExists) throw new ApplicationException("The given login already exists");

			var user = User.New(data.Name, data.Login);
			user.SetPassword(data.Password);

			await _db.User.AddAsync(user);

			return new CreateUserResult()
			{
				Id = user.Id,
				Name = user.Name,
				Login = user.Login,
				CreatedAt = user.CreatedAt
			};
		}

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

			if (user == null) return null;

			return new CreateUserResult()
			{
				Id = user.Id,
				Login = user.Login,
				Name = user.Name,
				CreatedAt = user.CreatedAt
			};
		}

		public async Task<AuthenticationResult> Authenticate(AuthenticationData data)
		{
			var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Login == data.Login && x.Password == data.Password);

			if (user == null) return null;

			return new AuthenticationResult()
			{
				UserId = user.Id,
				UserName = user.Name,
				Role = user.Role
			};
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

		public async Task ChangePassword(Guid userId, ChangePasswordData data)
		{
			var user = await _db.User.FirstOrDefaultAsync(x => x.Id == userId && x.Password == data.OldPassword);
			if (user == null) throw new ApplicationException("User not found");

			user.SetPassword(data.NewPassword);
		}

		public async Task Delete(Guid userId)
		{
			var user = await _db.User.FindAsync(userId);
			if (user == null) throw new ApplicationException("User not found");

			_db.User.Remove(user);
		}

		public async Task AlterUserRole(AlterUserRoleData data)
		{
			var authenticatedUser = await _db.User.FindAsync(data.AuthenticatedUser);
			if (authenticatedUser == null) throw new ApplicationException("Authenticated user not found");
			if (authenticatedUser.Role != UserRole.Admin) throw new InvalidOperationException("The authenticated user has no rights to alter user's role.");

			var targetUser = await _db.User.FindAsync(data.TargetUser);
			if (targetUser == null) throw new ApplicationException("Target user not found");

			authenticatedUser.AlterUserRole(targetUser, data.NewRole);
		}

		#endregion Methods
	}
}