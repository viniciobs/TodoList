using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repository.Tests.Seed
{
	public class FakeContext : IDisposable
	{
		public ApplicationContext DbContext { get; private set; }

		public FakeContext()
		{
			var fakeDbName = Guid.NewGuid().ToString("N").ToLower().Substring(0, 10);
			var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase(databaseName: fakeDbName).Options;
			DbContext = new ApplicationContext(options);
		}

		public void Dispose()
		{
			DbContext.Dispose();
		}
	}
}
