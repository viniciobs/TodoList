using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repository.Tests.Seed
{
    public class FakeContext
    {
        public ApplicationContext DbContext { get; private set; }

        public FakeContext()
        {
            var fakeDbName = Guid.NewGuid().ToString("N").ToLowerInvariant()[..10];
            var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase(databaseName: fakeDbName).Options;

            DbContext = new ApplicationContext(options);
        }
    }
}