using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Accounts;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : _Commom.Repository, IAccountRepository
    {
        public AccountRepository(ApplicationContext context)
            : base(context)
        { }

        public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationData data)
        {
            if (data == null) throw new MissingArgumentsException(nameof(data));
            if (string.IsNullOrEmpty(data.Login)) throw new MissingArgumentsException(nameof(data.Login));
            if (string.IsNullOrEmpty(data.Password)) throw new MissingArgumentsException(nameof(data.Password));

            var user = await _db.User.AsNoTracking().SingleOrDefaultAsync(x => x.Login == data.Login && x.Password == data.Password);
            if (user == null) throw new NotFoundException(typeof(User), "Invalid credentials");

            return new AuthenticationResult()
            {
                UserId = user.Id,
                Name = user.Name,
                Login = user.Login,
                IsActive = user.IsActive,
                Role = user.Role
            };
        }

        public void ChangePassword(User user, ChangePasswordData data)
        {
            if (user == null) throw new UnauthorizeException();
            if (data == null) throw new MissingArgumentsException(nameof(data));
            if (string.IsNullOrEmpty(data.OldPassword?.Trim())) throw new MissingArgumentsException(data.OldPassword);
            if (string.IsNullOrEmpty(data.NewPassword?.Trim())) throw new MissingArgumentsException(data.NewPassword);
            if (user.Password != data.OldPassword?.Trim()) throw new RuleException("Old password is wrong");

            user.SetPassword(data.NewPassword);
            _db.Update(user);
        }

        public async Task<Guid> CreateAsync(CreateAccountData data)
        {
            if (data == null) throw new MissingArgumentsException(nameof(data));
            if (string.IsNullOrEmpty(data.Name)) throw new MissingArgumentsException(nameof(data.Name));
            if (string.IsNullOrEmpty(data.Password)) throw new MissingArgumentsException(nameof(data.Password));

            await ValidateLogin(data.Login);

            var user = User.New(data.Name, data.Login);
            user.SetPassword(data.Password);

            await _db.User.AddAsync(user);

            return user.Id;
        }

        private async Task ValidateLogin(string login)
        {
            if (string.IsNullOrEmpty(login?.Trim())) throw new MissingArgumentsException(nameof(login));

            var loginExists = await _db.User.AnyAsync(x => x.Login == login);
            if (loginExists) throw new RuleException("The given login already exists");
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _db.User.FindAsync(userId);
            if (user == null) throw new NotFoundException(typeof(User));

            _db.User.Remove(user);
        }

        public async Task AlterStatusAsync(Guid userId, bool active)
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

        public async Task EditAsync(User user, EditData data)
        {
            if (data == null) throw new MissingArgumentsException(nameof(data));
            if (user == null) throw new UnauthorizeException();

            bool hasAnyChange = !string.IsNullOrEmpty(data.Name?.Trim()) || !string.IsNullOrEmpty(data.Login?.Trim());
            hasAnyChange &= user.Login != data.Login || user.Name != data.Name;
            if (!hasAnyChange) return;

            bool alterLogin = data.Login != null && user.Login != data.Login;
            if (alterLogin)
            {
                await ValidateLogin(data.Login);
                user.SetLogin(data.Login);
            }

            bool alterName = data.Name != null && data.Name != user.Name;
            if (alterName)
            {
                user.SetName(data.Name);
            }

            _db.User.Update(user);
        }
    }
}