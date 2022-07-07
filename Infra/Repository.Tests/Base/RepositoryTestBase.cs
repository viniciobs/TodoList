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
        protected readonly IUserRepository userRepository;
        protected readonly ITaskRepository taskRepository;
        protected readonly ITaskCommentRepository commentRepository;

        public RepositoryTestBase()
        {
            context = new FakeContext().DbContext;

            accountRepository = new AccountRepository(context);
            paginationRepository = new PaginationRepository(context);
            userRepository = new UserRepository(context, paginationRepository);
            taskRepository = new TaskRepository(context, paginationRepository);
            commentRepository = new TaskCommentRepository(context, paginationRepository);
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

        protected void EnsureUserIsActive(User user)
        {
            user.Activate();
        }
    }
}