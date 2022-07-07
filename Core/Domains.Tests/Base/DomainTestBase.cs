using System;
using Task = Domains.User.Task;

namespace Domains.Tests
{
    public abstract class DomainTestBase
    {
        protected readonly User normalUser;
        protected readonly User adminUser;

        public DomainTestBase()
        {
            normalUser = GenerateRandomUser();
            adminUser = GenerateAdminUser();
        }

        protected User GenerateRandomUser() => User.New(GenerateRandomString(), GenerateRandomString());

        protected User GenerateAdminUser() => User.NewAdmin();

        protected string GenerateRandomString() => Guid.NewGuid().ToString("N").ToLower().Substring(0, 10);
    }
}