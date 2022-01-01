using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domains.Tests
{
	[TestClass]
	public class TasksTest : DomainTestBase
	{
		[TestMethod]
		public void TestSetTaskThrowsMissingArgumentsException()
		{
			// Assert
			var user = GenerateRandomUser();

			// Act and assert
			Assert.ThrowsException<MissingArgumentsException>(() => user.AssignTask(user, NullTaskDescription));
			Assert.ThrowsException<MissingArgumentsException>(() => user.AssignTask(user, EmptyTaskDescription));
			Assert.ThrowsException<MissingArgumentsException>(() => user.AssignTask(NullUser, ValidTaskDescription));
		}

		[TestMethod]
		public void TestSetTaskOk()
		{
			// Act
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();
			var task = creatorUser.AssignTask(targetUser, ValidTaskDescription);

			// Assert
			Assert.IsTrue(creatorUser.CreatedTasks.Count == 1);
			Assert.IsTrue(targetUser.TargetTasks.Count == 1);
			Assert.IsTrue(creatorUser.TargetTasks.Count == 0);
			Assert.IsTrue(targetUser.CreatedTasks.Count == 0);
			Assert.AreEqual(ValidTaskDescription, task.Description);
			Assert.AreEqual(targetUser, task.TargetUser);
			Assert.AreEqual(creatorUser, task.CreatorUser);
		}

		[TestMethod]
		public void TestSetMultipleTasksOk()
		{
			// Arrange
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();

			var taskDescription1 = GenerateRandomString();
			var taskDescription2 = GenerateRandomString();

			// Act
			var task1 = creatorUser.AssignTask(targetUser, taskDescription1);
			var task2 = creatorUser.AssignTask(targetUser, taskDescription2);

			// Assert
			Assert.IsTrue(creatorUser.CreatedTasks.Count == 2);
			Assert.IsTrue(targetUser.TargetTasks.Count == 2);
			Assert.IsTrue(creatorUser.TargetTasks.Count == 0);
			Assert.IsTrue(targetUser.CreatedTasks.Count == 0);
			Assert.AreEqual(taskDescription1, task1.Description);
			Assert.AreEqual(taskDescription2, task2.Description);
			Assert.AreEqual(targetUser, task1.TargetUser);
			Assert.AreEqual(targetUser, task2.TargetUser);
		}

		[TestMethod]
		public void TestSetMultipleUsersTasksOk()
		{
			// Arrange
			var taskDescription1 = GenerateRandomString();
			var user1 = GenerateRandomUser();

			var taskDescription2 = GenerateRandomString();
			var user2 = GenerateRandomUser();

			// Act
			var task1 = user1.AssignTask(user2, taskDescription1);
			var task2 = user2.AssignTask(user1, taskDescription2);

			// Assert
			Assert.IsTrue(user1.CreatedTasks.Count == 1);
			Assert.IsTrue(user1.TargetTasks.Count == 1);
			Assert.IsTrue(user2.TargetTasks.Count == 1);
			Assert.IsTrue(user2.CreatedTasks.Count == 1);
			Assert.AreEqual(taskDescription1, task1.Description);
			Assert.AreEqual(taskDescription2, task2.Description);
			Assert.AreEqual(user2, task1.TargetUser);
			Assert.AreEqual(user1, task2.TargetUser);
			Assert.AreEqual(user1, task1.CreatorUser);
			Assert.AreEqual(user2, task2.CreatorUser);
		}

		[TestMethod]
		public void TestFinishTaskThrowRuleException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(user, ValidTaskDescription);
			user.FinishTask(task);

			// Act and assert
			Assert.ThrowsException<RuleException>(() => user.FinishTask(task));
		}

		[TestMethod]
		public void TestFinishTaskTrhowMissingArgumentsException()
		{
			// Assert
			var user = GenerateRandomUser();			

			// Act and assert
			Assert.ThrowsException<MissingArgumentsException>(() => user.FinishTask(NullTask));
		}

		[TestMethod]
		public void TestFinishTaskThrowPermissionException()
		{
			//Act and assert
			Assert.ThrowsException<PermissionException>(() => GenerateRandomUser().FinishTask(GenerateRandomTask()));
		}

		[TestMethod]
		public void TestUserFinishTaskOk()
		{
			// Arrange
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();

			var task1 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			creatorUser.FinishTask(task1);

			var task2 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task2);

			//Act
			Assert.IsNotNull(task1.CompletedAt);
			Assert.IsNotNull(task2.CompletedAt);
		}

		[TestMethod]
		public void TestReopenTaskThrowsMissingArgumentsException()
		{
			//Act and assert
			Assert.ThrowsException<MissingArgumentsException>(() => GenerateRandomUser().ReopenTask(NullTask));
		}

		[TestMethod]
		public void TestReopenTaskThrowRuleException()
		{
			// Arrange
			var task = GenerateRandomTask();
			var creatorUser = task.CreatorUser;
			var targetUser = task.TargetUser;

			//Act and assert
			Assert.ThrowsException<RuleException>(() => creatorUser.ReopenTask(task));
			Assert.ThrowsException<RuleException>(() => targetUser.ReopenTask(task));
		}

		[TestMethod]
		public void TestReopenTaskThrowPermissionException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(user, ValidTaskDescription);
			user.FinishTask(task);

			//Act and assert
			Assert.ThrowsException<PermissionException>(() => GenerateRandomUser().ReopenTask(task));
		}

		public void TestReopenTaskOk()
		{
			// Act
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();

			var task1 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task1);
			targetUser.ReopenTask(task1);

			var task2 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task2);
			creatorUser.ReopenTask(task2);

			var task3 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			creatorUser.FinishTask(task3);
			targetUser.ReopenTask(task3);

			var task4 = creatorUser.AssignTask(targetUser, ValidTaskDescription);
			creatorUser.FinishTask(task4);
			creatorUser.ReopenTask(task4);

			// Assert
			Assert.IsNull(task1.CompletedAt);
			Assert.IsNull(task2.CompletedAt);
			Assert.IsNull(task3.CompletedAt);
			Assert.IsNull(task4.CompletedAt);
		}
	}
}