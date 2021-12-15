using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Accounts;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;
using Repository._Commom;

namespace Repository
{
	public class AccountRepository : _Commom.Repository, IAccountRepository
	{
		#region Constructor

		public AccountRepository(ApplicationContext context)
			: base(context)
		{ }		

		#endregion Constructor

		#region Methods

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

		public async Task Create(CreateAccountData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (string.IsNullOrEmpty(data.Name)) throw new MissingArgumentsException(nameof(data.Name));
			if (string.IsNullOrEmpty(data.Login.Trim())) throw new MissingArgumentsException(nameof(data.Login));
			if (string.IsNullOrEmpty(data.Password)) throw new MissingArgumentsException(nameof(data.Password));

			var loginExists = await _db.User.AnyAsync(x => x.Login == data.Login);
			if (loginExists) throw new RuleException("The given login already exists");

			var user = User.New(data.Name, data.Login);
			user.SetPassword(data.Password);

			await _db.User.AddAsync(user);
		}

		public async Task Delete(Guid userId)
		{
			var user = await _db.User.FindAsync(userId);
			if (user == null) throw new NotFoundException(typeof(User));

			_db.User.Remove(user);
		}

		public async Task AlterStatus(Guid userId, bool active)
		{
			var user = await _db.User.FindAsync(userId);
			if (user == null) throw new NotFoundException(typeof(User));

			if (active)
			{
				user.Activate();
			}
			else
			{
				await _db.Entry(user).Collection(x => x.TargetTasks).LoadAsync();
				user.Deactivate();
			}

			_db.User.Update(user);
		}

		#endregion Methods
	}
}