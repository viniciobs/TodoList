using DataAccess;
using Domains;
using Domains.Tests;
using Repository.DTOs.Accounts;
using Repository.Tests.Seed;

namespace Repository.Tests.Base
{
    public abstract class RepositoryTestBase : DomainTestBase
    {
        protected readonly ApplicationContext context;
        protected readonly AccountRepository repository;

        public RepositoryTestBase()
        {
            context = new FakeContext().DbContext;
            repository = new AccountRepository(context);
        }

        protected CreateAccountData GenerateValidCreateAccountData()
        {
            var randomUser = GenerateRandomUser();

            var randomValidAccountData = new CreateAccountData()
            {
                Name = randomUser.Name,
                Login = randomUser.Login,
                Password = GenerateRandomString()
            };

            return randomValidAccountData;
        }

        protected User GenerateDeactivatedUser()
        {
            var randomUser = GenerateRandomUser();
            randomUser.Deactivate();

            return randomUser;
        }
    }
}