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

        protected static User GenerateRandomUser()
        {
          var user = User.New("Normal User", GenerateRandomString());
          user.SetPassword(GenerateRandomString());

          return user;
        }

        protected static User GenerateAdminUser() 
        {
            var user = User.NewAdmin();
            user.SetPassword(GenerateRandomString());
            
            return user;
        }

        protected static string GenerateRandomString() => Guid.NewGuid().ToString("N").ToLower()[..10];
    }
}