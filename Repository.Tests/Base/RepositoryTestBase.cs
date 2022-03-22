using DataAccess;
using Domains;
using Domains.Tests;
using Repository._Commom;
using Repository.DTOs.Accounts;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using Repository.Tests.Seed;

namespace Repository.Tests.Base
{
    public abstract class RepositoryTestBase : DomainTestBase
    {
        protected readonly ApplicationContext context;
        protected readonly IAccountRepository accountRepository;
        protected readonly IPaginationRepository paginationRepository;
        protected IUserRepository userRepository;

        public RepositoryTestBase()
        {
            context = new FakeContext().DbContext;

            accountRepository = new AccountRepository(context);
            paginationRepository = new PaginationRepository(context);
            userRepository = new UserRepository(context, paginationRepository);
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