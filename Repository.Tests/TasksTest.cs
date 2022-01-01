using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs.Tasks;
using Repository.Tests.Base;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class TasksTest : RepositoryTestBase
	{
		[TestMethod]
		public void TestAssignThrowMissingArgumentsException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var repository = new TaskRepository(context, paginationRepository);

			// Act
			AssignTaskData data = null;			
			Exception resultException;

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data =new AssignTaskData();			

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act			
			data.Description = EmptyTaskDescription;
			data.CreatorUser = null;
			data.TargetUser = null;

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Description = NullTaskDescription;
			data.CreatorUser = GenerateRandomUser();
			data.TargetUser = GenerateRandomUser();

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Description = ValidTaskDescription;
			data.CreatorUser = null;
			data.TargetUser = null;

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Description = ValidTaskDescription;
			data.CreatorUser = GenerateRandomUser();
			data.TargetUser = null;

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Description = ValidTaskDescription;
			data.CreatorUser = null;
			data.TargetUser = GenerateRandomUser();

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());
		}

		[TestMethod]
		public void TestAssignThrowInactiveUser()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var repository = new TaskRepository(context, paginationRepository);

			// Act
			AssignTaskData data;
			Exception resultException;

			data = new AssignTaskData()
			{
				CreatorUser = GenerateRandomUser(),
				TargetUser = GenerateDeactivatedUser(),
				Description = ValidTaskDescription
			};

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(RuleException), resultException.GetType());

			// Act			
			data = new AssignTaskData()
			{
				CreatorUser = GenerateDeactivatedUser(),
				TargetUser = GenerateRandomUser(),
				Description = ValidTaskDescription
			};

			resultException = repository.AssignAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(RuleException), resultException.GetType());
		}

		[TestMethod]
		public void TestAssignOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var creatorUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var targetUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var creatorUser = context.User.Single(x => x.Id == creatorUserId);
			var targetUser = context.User.Single(x => x.Id == targetUserId);
			var taskDescription = ValidTaskDescription;

			var data = new AssignTaskData()
			{
				CreatorUser = creatorUser,
				TargetUser = targetUser,
				Description = taskDescription
			};

			var taskResult = taskRepository.AssignAsync(data).Result;

			// Assert
			Assert.IsNull(taskResult.CompletedAt);
			Assert.AreEqual(creatorUserId, taskResult.CreatorUser.Id);
			Assert.AreEqual(targetUserId, taskResult.TargetUser.Id);
			Assert.AreEqual(taskDescription, taskResult.Description);
		}
	}
}
