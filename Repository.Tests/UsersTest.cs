using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Accounts;
using Repository.DTOs.Users;
using Repository.Tests.Base;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class UsersTest : RepositoryTestBase
	{
		[TestMethod]
		public void TestGetUserByFilterOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = GenerateRandomString()
			}).Wait();

			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "kelly",
				Name = "Kelly Osbourne",
				Password = GenerateRandomString()
			}).Wait();

			accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "niceVoice",
				Name = "Eddie Vedder",
				Password = GenerateRandomString()
			}).Wait();			

			var coreyId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "cmft",
				Name = "Corey Taylor",
				Password = GenerateRandomString()
			}).Result;

			var derrickId = accountRepository.CreateAsync(new CreateAccountData()
			{
				Login = "wrargh",
				Name = "Derrick Green",
				Password = GenerateRandomString()
			}).Result;

			// Act
			accountRepository.SaveChangesAsync().Wait();

			accountRepository.AlterStatusAsync(coreyId, false).Wait();
			accountRepository.AlterStatusAsync(derrickId, false).Wait();

			accountRepository.SaveChangesAsync().Wait();			

			UserFilter filter;
			PaginationResult<UserResult> result;

			filter = new UserFilter()
			{
				Name = "Osbourne"
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Kelly Osbourne", "Ozzy Osbourne" }));

			// Act
			filter = new UserFilter()
			{
				Name = "gReEn"
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Derrick Green" }));


			// Act
			filter = new UserFilter()
			{
				Login = "CMFT"
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Corey Taylor" }));

			// Act
			filter = new UserFilter()
			{
				Login = "CMFT",
				Name = "Derrick"
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsTrue(result.Data.Count() == 0);

			// Act
			filter = new UserFilter()
			{
				IsActive = false
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsTrue(Enumerable.SequenceEqual(result.Data.Select(x => x.Name), new[] { "Corey Taylor", "Derrick Green" }));

			// Act
			filter = new UserFilter()
			{
				IsActive = true
			};

			result = userRepository.GetAsync(filter).Result;

			// Assert
			Assert.IsFalse(result.Data.Select(x => x.Name).Intersect(new[] { "Corey Taylor", "Derrick Green" }).Any());

			context.Dispose();
		}

		[TestMethod]
		public void TestFindUserThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;			
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			var resultException = userRepository.FindAsync(default).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestFindUserOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			var someUserId = accountRepository.CreateAsync(GetValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait(); 
			
			var someUser = context.User.Single(x => x.Id == someUserId);			
			
			// Assert
			Assert.IsNotNull(userRepository.FindAsync(someUserId).Result);
			Assert.IsNotNull(userRepository.FindAsync(someUserId, true).Result);

			// Act
			var resultException = userRepository.FindAsync(someUserId, false).Exception.InnerException;
			
			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			// Act
			accountRepository.AlterStatusAsync(someUserId, false).Wait();
			accountRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNotNull(userRepository.FindAsync(someUserId, false).Result);

			// Act
			var anotherUserId = accountRepository.CreateAsync(GetValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var anotherUser = context.User.Single(x => x.Id == anotherUserId);

			var result = userRepository.FindAsync(anotherUserId).Result;

			// Assert
			Assert.AreNotEqual(result.Name, someUser.Name);
			Assert.AreEqual(result.Name, anotherUser.Name);

			context.Dispose();
		}

		[TestMethod]
		public void TestAlterUserRoleThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			Exception resultException;
			AlterUserRoleData data = null;
			
			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new AlterUserRoleData();
			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new AlterUserRoleData();
			data.TargetUser = default;
			data.AuthenticatedUser = Guid.NewGuid();
			
			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new AlterUserRoleData();
			data.AuthenticatedUser = default;
			data.TargetUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAlterUserRoleThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			var adminUser = User.NewAdmin();
			context.User.Add(adminUser);
			context.SaveChanges();
			
			Exception resultException;
			AlterUserRoleData data;

			data = new AlterUserRoleData
			{
				TargetUser = adminUser.Id,
				AuthenticatedUser = Guid.NewGuid()
			};

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			// Act
			data = new AlterUserRoleData();
			data.AuthenticatedUser = adminUser.Id;
			data.TargetUser = Guid.NewGuid();

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAlterUserRoleThrowPermissionException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			var userId = accountRepository.CreateAsync(GetValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();
			
			Exception resultException;
			AlterUserRoleData data;

			data = new AlterUserRoleData
			{
				AuthenticatedUser = userId,
				TargetUser = Guid.NewGuid()
			};

			resultException = userRepository.AlterUserRoleAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(PermissionException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAlterUserRoleOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);
			var userRepository = new UserRepository(context, paginationRepository);

			// Act
			var userId = accountRepository.CreateAsync(GetValidCreateAccountData()).Result;
			
			accountRepository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);
			
			// Assert
			Assert.AreEqual(user.Role, UserRole.Normal);

			// Act			
			var adminUser = User.NewAdmin();
			context.User.Add(adminUser);
			context.SaveChanges();
				
			var data = new AlterUserRoleData
			{
				AuthenticatedUser = adminUser.Id,
				TargetUser = userId
			};

			userRepository.AlterUserRoleAsync(data).Wait();
			userRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(user.Role, UserRole.Admin);

			context.Dispose();
		}
	}
}
