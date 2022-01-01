using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs._Commom;
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

			context.Dispose();
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

			context.Dispose();
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

			context.Dispose();
		}

		[TestMethod]
		public void TestFindThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;	
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act			
			var resultException = taskRepository.FindAsync(Guid.NewGuid(), Guid.NewGuid()).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestFindOk()
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
			
			taskRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNotNull(taskRepository.FindAsync(creatorUserId, taskResult.Id).Result);
			Assert.IsNotNull(taskRepository.FindAsync(targetUserId, taskResult.Id).Result);
			Assert.AreEqual(taskDescription, taskRepository.FindAsync(targetUserId, taskResult.Id).Result.Description);

			context.Dispose();
		}

		[TestMethod]
		public void TestFinishTaskThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			UserTask data = null;
			Exception resultException;

			resultException = taskRepository.FinishAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask();

			resultException = taskRepository.FinishAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask()
			{
				TaskId = default,
				User = null
			};

			resultException = taskRepository.FinishAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask()
			{
				TaskId = Guid.NewGuid(),
				User = null
			};

			resultException = taskRepository.FinishAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestFinishTaskThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var data = new UserTask()
			{
				User = GenerateRandomUser(),
				TaskId = Guid.NewGuid()
			};

			var resultException = taskRepository.FinishAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestFinishTaskOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var randomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var anotherRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var randomUser = context.User.Single(x => x.Id == randomUserId);
			var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);

			var data = new AssignTaskData()
			{
				CreatorUser = randomUser,
				TargetUser = anotherRandomUser,
				Description = GenerateRandomString()
			};

			var taskResult = taskRepository.AssignAsync(data).Result;

			taskRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNull(taskResult.CompletedAt);

			// Act
			taskRepository.FinishAsync(new UserTask()
			{
				TaskId = taskResult.Id,
				User = randomUser
			}).Wait();

			var task = context.Task.Single(x => x.Id == taskResult.Id);

			// ASsert
			Assert.IsNotNull(task.CompletedAt);

			context.Dispose();
		}

		[TestMethod]
		public void TestTaskReopenThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			UserTask data = null;
			Exception resultException;

			resultException = taskRepository.ReopenAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask();

			resultException = taskRepository.ReopenAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask()
			{
				TaskId = default,
				User = null
			};

			resultException = taskRepository.ReopenAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new UserTask()
			{
				TaskId = Guid.NewGuid(),
				User = null
			};

			resultException = taskRepository.ReopenAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestReopenTaskThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var data = new UserTask()
			{
				User = GenerateRandomUser(),
				TaskId = Guid.NewGuid()
			};

			var resultException = taskRepository.ReopenAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestReopenTaskOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var randomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var anotherRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var randomUser = context.User.Single(x => x.Id == randomUserId);
			var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);

			var assignData = new AssignTaskData()
			{
				CreatorUser = randomUser,
				TargetUser = anotherRandomUser,
				Description = GenerateRandomString()
			};

			var taskResult = taskRepository.AssignAsync(assignData).Result;

			taskRepository.SaveChangesAsync().Wait();

			var data = new UserTask()
			{
				TaskId = taskResult.Id,
				User = randomUser
			};

			User.Task task;

			taskRepository.FinishAsync(data).Wait();
			taskRepository.SaveChangesAsync().Wait();

			task = context.Task.Single(x => x.Id == taskResult.Id);

			// Assert
			Assert.IsNotNull(task.CompletedAt);

			// Act
			taskRepository.ReopenAsync(data).Wait();
			taskRepository.SaveChangesAsync().Wait();

			task = context.Task.Single(x => x.Id == taskResult.Id);

			// ASsert
			Assert.IsNull(task.CompletedAt);

			context.Dispose();
		}

		[TestMethod]
		public void TestGetByFilterOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var accountRepository = new AccountRepository(context);
			var paginationRepository = new PaginationRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);

			// Act
			var randomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var anotherRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var oneMoreRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var randomUser = context.User.Single(x => x.Id == randomUserId);
			var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);
			var oneMoreRandomUser = context.User.Single(x => x.Id == oneMoreRandomUserId);			

			taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = randomUser,
				TargetUser = anotherRandomUser,
				Description = GenerateRandomString()
			}).Wait();

			taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = randomUser,
				TargetUser = oneMoreRandomUser,
				Description = GenerateRandomString()
			}).Wait();

			var randomTask = taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = oneMoreRandomUser,
				TargetUser = randomUser,
				Description = GenerateRandomString()
			}).Result;

			taskRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { CreatorUser = randomUser.Id }).Result.Data.Count(), 2);
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { CreatorUser = anotherRandomUser.Id }).Result.Data.Count(), 0);
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.OR }).Result.Data.Count(), 3);
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.AND }).Result.Data.Count(), 0);


			// Act
			var anotherRandomTask = taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = oneMoreRandomUser,
				TargetUser = oneMoreRandomUser,
				Description = GenerateRandomString()
			}).Result;

			taskRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = oneMoreRandomUser.Id, CreatorUser = oneMoreRandomUser.Id, UserFilter = FilterHelper.AND }).Result.Data.Count(), 1);
			Assert.AreEqual(taskRepository.GetAsync(null).Result.Data.Count(), 4);

			// Act
			taskRepository.FinishAsync(new UserTask() { TaskId = randomTask.Id, User = randomUser }).Wait();
			taskRepository.FinishAsync(new UserTask() { TaskId = anotherRandomTask.Id, User = oneMoreRandomUser}).Wait();

			taskRepository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = true }).Result.Data.Count(), 2);
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = false }).Result.Data.Count(), 2);
			Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = null }).Result.Data.Count(), 4);

			context.Dispose();
		}
	}
}
