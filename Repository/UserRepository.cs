using DataAccess;
using Domains;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Users;
using Repository.Exceptions;
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

		public async Task<CreateUserResult> Create(CreateUserData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.Name)) throw new MissingArgumentsException(nameof(data.Name));
			if (string.IsNullOrEmpty(data.Login)) throw new MissingArgumentsException(nameof(data.Login));
			if (string.IsNullOrEmpty(data.Password)) throw new MissingArgumentsException(nameof(data.Password));

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
			if (user == null) throw new NotFoundException(typeof(User));

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
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.Login)) throw new MissingArgumentsException(nameof(data.Login));
			if (string.IsNullOrEmpty(data.Password)) throw new MissingArgumentsException(nameof(data.Password));

			var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Login == data.Login && x.Password == data.Password);
			if (user == null) throw new NotFoundException(typeof(User), "Invalid credentials");

			return new AuthenticationResult()
			{
				UserId = user.Id,
				UserName = user.Name,
				Role = user.Role
			};
		}

		public async Task ChangePassword(Guid userId, ChangePasswordData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.OldPassword)) throw new MissingArgumentsException(data.OldPassword);
			if (string.IsNullOrEmpty(data.NewPassword)) throw new MissingArgumentsException(data.NewPassword);

			var user = await _db.User.FirstOrDefaultAsync(x => x.Id == userId && x.Password == data.OldPassword);
			if (user == null) throw new NotFoundException(typeof(User));

			user.SetPassword(data.NewPassword);
		}

		public async Task Delete(Guid userId)
		{
			var user = await _db.User.FindAsync(userId);
			if (user == null) throw new NotFoundException(typeof(User));

			_db.User.Remove(user);
		}

		public async Task AlterUserRole(AlterUserRoleData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));

			var authenticatedUser = await _db.User.FindAsync(data.AuthenticatedUser);
			if (authenticatedUser == null) throw new NotFoundException(typeof(User), "Authenticated user not found");
			if (authenticatedUser.Role != UserRole.Admin) throw new InvalidOperationException("The authenticated user has no rights to alter user's role.");

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