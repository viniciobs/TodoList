using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Tests.Base;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class TaskCommentsTest : RepositoryTestBase
	{
		[TestMethod]
		public void TestAddCommentThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var taskCommentRepository = new TaskCommentRepository(context, paginationRepository);

			// Act
			Exception resultException;
			TaskCommentData data = null;

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new TaskCommentData();

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.User = null;
			data.TaskId = Guid.NewGuid();
			data.Comment = ValidComment;

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.User = GenerateRandomUser();
			data.TaskId = default;
			data.Comment = ValidComment;

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.User = GenerateRandomUser();
			data.TaskId = default;
			data.Comment = EmptyComment;

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.User = GenerateRandomUser();
			data.TaskId = default;
			data.Comment = NullComment;

			resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAddCommentThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var taskCommentRepository = new TaskCommentRepository(context, paginationRepository);

			// Act
			var data = new TaskCommentData()
			{
				Comment = ValidComment,
				User = GenerateRandomUser(),
				TaskId = Guid.NewGuid()
			};

			var resultException = taskCommentRepository.AddCommentAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAddCommentOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);
			var taskCommentRepository = new TaskCommentRepository(context, paginationRepository);

			// Act
			var randomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			
			accountRepository.SaveChangesAsync().Wait();

			var randomUser = context.User.Single(x => x.Id == randomUserId);
			
			var assingTaskData = new AssignTaskData()
			{
				CreatorUser = randomUser,
				TargetUser = randomUser,
				Description = ValidTaskDescription
			};

			var task = taskRepository.AssignAsync(assingTaskData).Result;

			taskRepository.SaveChangesAsync().Wait();

			var data = new TaskCommentData()
			{
				Comment = ValidComment,
				User = randomUser,
				TaskId = task.Id
			};

			var result = taskCommentRepository.AddCommentAsync(data).Result;

			taskCommentRepository.SaveChangesAsync().Wait();

			var comment = context.TaskComment.Single();

			// Assert
			Assert.AreEqual(result.Comment, comment.Text);
			Assert.AreEqual(result.UserId, comment.CreatedByUserId);
			Assert.AreNotEqual(result.CreatedAt, default);

			context.Dispose();
		}

		[TestMethod]
		public void TestGetByFilterOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var paginationRepository = new PaginationRepository(context);
			var accountRepository = new AccountRepository(context);
			var taskRepository = new TaskRepository(context, paginationRepository);
			var taskCommentRepository = new TaskCommentRepository(context, paginationRepository);

			// Act
			var randomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
			var anotherRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

			accountRepository.SaveChangesAsync().Wait();

			var randomUser = context.User.Single(x => x.Id == randomUserId);
			var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);


			var randomTask = taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = anotherRandomUser,
				TargetUser = randomUser,
				Description = ValidTaskDescription
			}).Result;

			var anotherRandomTask = taskRepository.AssignAsync(new AssignTaskData()
			{
				CreatorUser = anotherRandomUser,
				TargetUser = randomUser,
				Description = ValidTaskDescription
			}).Result;

			taskRepository.SaveChangesAsync().Wait();

			var data = new TaskCommentData()
			{
				Comment = ValidComment,
				User = randomUser,
				TaskId = randomTask.Id
			};

			taskCommentRepository.AddCommentAsync(data).Wait();

			data = new TaskCommentData()
			{
				Comment = GenerateRandomString(),
				User = anotherRandomUser,
				TaskId = randomTask.Id
			};

			taskCommentRepository.AddCommentAsync(data).Wait();

			data = new TaskCommentData()
			{
				Comment = GenerateRandomString(),
				User = randomUser,
				TaskId = anotherRandomTask.Id
			};

			taskCommentRepository.AddCommentAsync(data).Wait();
			taskCommentRepository.SaveChangesAsync().Wait();

			PaginationResult<TaskCommentResult> result;
			TaskCommentFilter filter = null;

			result = taskCommentRepository.GetAsync(filter).Result;

			// Assert
			Assert.AreEqual(result.Data.Count(), 3);

			// Act
			filter = new TaskCommentFilter()
			{
				TaskId = randomTask.Id
			};

			result = taskCommentRepository.GetAsync(filter).Result;

			// Assert
			Assert.AreEqual(result.Data.Count(), 2);

			// Act
			filter = new TaskCommentFilter()
			{
				TaskId = anotherRandomTask.Id
			};

			result = taskCommentRepository.GetAsync(filter).Result;

			// Assert
			Assert.AreEqual(result.Data.Count(), 1);

			// Act
			var date = DateTime.Today.AddDays(-2);

			var comment1 = context.TaskComment.First();
			comment1.AlterCreatedAt(date);

			var comment2 = context.TaskComment.Last();
			comment2.AlterCreatedAt(date);

			context.TaskComment.UpdateRange(comment1, comment2);
			context.SaveChanges();

			filter = new TaskCommentFilter()
			{
				CreatedBetween = new Period(date.AddHours(-1), date.AddHours(1))				
			};

			result = taskCommentRepository.GetAsync(filter).Result;

			// Assert
			Assert.AreEqual(result.Data.Count(), 2);

			// Act
			filter = new TaskCommentFilter()
			{
				CreatedBetween = new Period(DateTime.Today, null)
			};

			result = taskCommentRepository.GetAsync(filter).Result;

			// Assert
			Assert.AreEqual(result.Data.Count(), 1);

			context.Dispose();
		}
	}
}
