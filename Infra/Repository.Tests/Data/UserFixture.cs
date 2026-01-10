using System;
using System.Threading.Tasks;
using Domains;
using Repository.DTOs.Accounts;
using Repository.Interfaces;
using Repository.Tests.Base;
using Xunit;

namespace Repository.Tests.Data
{
    public class UserFixture : RepositoryTestBase, IAsyncLifetime
    {
        private Guid coreyId;

        public Guid GetCoreyId() => coreyId;
        public User GetAdminUser() => adminUser;
        public User GetNormalUser() => normalUser;

        public IUserRepository GetUserRepository() => userRepository;

        public Task DisposeAsync() => 
            Task.CompletedTask;

        public async Task InitializeAsync()
        {
            adminUser.Activate();
            context.User.Add(adminUser);

            normalUser.Activate();
            context.User.Add(normalUser);

            await accountRepository.CreateAsync(new CreateAccountData { Login = "princeOfDarkness", Name = "Ozzy Osbourne", Password = GenerateRandomString() });
            await accountRepository.CreateAsync(new CreateAccountData { Login = "kelly", Name = "Kelly Osbourne", Password = GenerateRandomString() });
            await accountRepository.CreateAsync(new CreateAccountData { Login = "niceVoice", Name = "Eddie Vedder", Password = GenerateRandomString() });

            coreyId = await accountRepository.CreateAsync(new CreateAccountData { Login = "cmft", Name = "Corey Taylor", Password = GenerateRandomString() });
            var derrickId = await accountRepository.CreateAsync(new CreateAccountData { Login = "wrargh", Name = "Derrick Green", Password = GenerateRandomString() });

            await accountRepository.SaveChangesAsync();

            await accountRepository.AlterStatusAsync(coreyId, active: false);
            await accountRepository.AlterStatusAsync(derrickId, active: false);

            await accountRepository.SaveChangesAsync();
        }
    }
}