using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Accounts;
using Repository.DTOs.Users;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class UsersTest
	{
		[TestMethod]
		public void TestGetUserByFilterOk()
		{
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);			

			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			}).Wait();

			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "kelly",
				Name = "Kelly Osbourne",
				Password = "1234"
			}).Wait();

			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "niceVoice",
				Name = "Eddie Vedder",
				Password = "1234"
			}).Wait();			

			var coreyId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "cmft",
				Name = "Corey Taylor",
				Password = "1234"
			}).Result;

			var derrickId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "wrargh",
				Name = "Derrick Green",
				Password = "1234"
			}).Result;

			accountRepository.SaveChangesAsync().Wait();

			accountRepository.AlterStatusAsync(coreyId, false).Wait();
			accountRepository.AlterStatusAsync(derrickId, false).Wait();

			accountRepository.SaveChangesAsync().Wait();

			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			UserFilter filter;
			PaginationResult<UserResult> result;

			filter = new UserFilter()
			{
				Name = "Osbourne"
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Kelly Osbourne", "Ozzy Osbourne" }));

			filter = new UserFilter()
			{
				Name = "gReEn"
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Derrick Green" }));

			filter = new UserFilter()
			{
				Login = "CMFT"
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Corey Taylor" }));

			filter = new UserFilter()
			{
				Login = "CMFT",
				Name = "Derrick"
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsTrue(result.Data.Count() == 0);

			filter = new UserFilter()
			{
				IsActive = false
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Corey Taylor", "Derrick Green" }));

			filter = new UserFilter()
			{
				IsActive = true
			};

			result = userRepository.GetAsync(filter).Result;

			Assert.IsFalse(result.Data.Select(x => x.Name).Intersect(new[] { "Corey Taylor", "Derrick Green" }).Any());
		}

		[TestMethod]
		public void TestFindUserThrowMissingArgumentException()
		{
			var context = new FakeContext().DbContext;			
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			var resultException = userRepository.FindAsync(default).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());
		}

		[TestMethod]
		public void TestFindUserOk()
		{
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);

			var ozzyId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			}).Result;

			accountRepository.SaveChangesAsync().Wait();

			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			Assert.IsNotNull(userRepository.FindAsync(ozzyId).Result);
			Assert.IsNotNull(userRepository.FindAsync(ozzyId, true).Result);

			var resultException = userRepository.FindAsync(ozzyId, false).Exception.InnerException;
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			accountRepository.AlterStatusAsync(ozzyId, false).Wait();
			accountRepository.SaveChangesAsync().Wait();

			Assert.IsNotNull(userRepository.FindAsync(ozzyId, false).Result);

			var coreyId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "cmft",
				Name = "Corey Taylor",
				Password = "1234"
			}).Result;

			accountRepository.SaveChangesAsync().Wait();

			var result = userRepository.FindAsync(coreyId).Result;
			Assert.AreNotEqual(result.Name, "Ozzy Osbourne");
			Assert.AreEqual(result.Name, "Corey Taylor");
		}

		[TestMethod]
		public void TestAlterUserRoleThrowMissingArgumentException()
		{
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			Exception resultException;
			AlterUserRoleData data = null;
			
			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new AlterUserRoleData();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new AlterUserRoleData();
			data.TargetUser = default;
			data.AuthenticatedUser = Guid.NewGuid();
			
			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new AlterUserRoleData();
			data.AuthenticatedUser = default;
			data.TargetUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());
		}

		[TestMethod]
		public void TestAlterUserRoleThrowNotFoundException()
		{
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);

			var adminUser = User.NewAdmin();
			context.User.Add(adminUser);
			context.SaveChanges();

			var userRepository = new UserRepository(context, paginationRepository);

			Exception resultException;
			AlterUserRoleData data;			

			data = new AlterUserRoleData();
			data.TargetUser = adminUser.Id;
			data.AuthenticatedUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			data = new AlterUserRoleData();
			data.AuthenticatedUser = adminUser.Id;
			data.TargetUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());
		}

		[TestMethod]
		public void TestAlterUserRoleThrowPermissionException()
		{
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);

			var ozzyId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			}).Result;

			accountRepository.SaveChangesAsync().Wait();

			var userRepository = new UserRepository(context, paginationRepository);

			Exception resultException;
			AlterUserRoleData data;

			data = new AlterUserRoleData();
			data.AuthenticatedUser = ozzyId;
			data.TargetUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(PermissionException), resultException.GetType());
		}

		[TestMethod]
		public void TestAlterUserRoleOk()
		{
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);

			var userId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			}).Result;

			var adminUser = User.NewAdmin();
			context.User.Add(adminUser);
			context.SaveChanges();

			var user = context.User.Single(x => x.Id == userId);

			Assert.AreEqual(user.Role, UserRole.Normal);

			var userRepository = new UserRepository(context, paginationRepository);

			AlterUserRoleData data;

			data = new AlterUserRoleData();
			data.AuthenticatedUser = adminUser.Id;
			data.TargetUser = userId;

			userRepository.AlterUserRoleAsync(data).Wait();

			Assert.AreEqual(user.Role, UserRole.Admin);
		}
	}
}
