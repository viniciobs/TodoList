using Domains.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security;

namespace Tests
{
	[TestClass]
	public class TasksTest : TestBase
	{
		[TestMethod]
		public void TestSetTaskThrowsNullException()
		{
			// Assert
			var user = GenerateRandomUser();

			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => user.SetTask(user, NullTaskDescription));
			Assert.ThrowsException<ArgumentNullException>(() => user.SetTask(user, EmptyTaskDescription));
			Assert.ThrowsException<ArgumentNullException>(() => user.SetTask(NullUser, ValidTaskDescription));
		}

		[TestMethod]
		public void TestSetTaskOk()
		{
			// Act
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();
			var task = creatorUser.SetTask(targetUser, ValidTaskDescription);

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
			var task1 = creatorUser.SetTask(targetUser, taskDescription1);
			var task2 = creatorUser.SetTask(targetUser, taskDescription2);

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
			var task1 = user1.SetTask(user2, taskDescription1);
			var task2 = user2.SetTask(user1, taskDescription2);

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
		public void TestFinishTaskThrowInvalidOperationException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(user, ValidTaskDescription);
			user.FinishTask(task);

			// Act and assert
			Assert.ThrowsException<InvalidOperationException>(() => user.FinishTask(task));
		}

		[TestMethod]
		public void TestFinishTaskTrhowNullException()
		{
			// Assert
			var user = GenerateRandomUser();

			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => user.FinishTask(NullTask));
			Assert.ThrowsException<ArgumentNullException>(() => user.FinishTask(NullTask));
		}

		[TestMethod]
		public void TestFinishTaskThrowSecurityException()
		{
			//Act and assert
			Assert.ThrowsException<SecurityException>(() => GenerateRandomUser().FinishTask(GenerateRandomTask()));
		}

		[TestMethod]
		public void TestUserFinishTaskOk()
		{
			// Arrange
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();

			var task1 = creatorUser.SetTask(targetUser, ValidTaskDescription);
			creatorUser.FinishTask(task1);

			var task2 = creatorUser.SetTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task2);

			//Act
			Assert.IsNotNull(task1.CompletedAt);
			Assert.IsNotNull(task2.CompletedAt);
		}

		[TestMethod]
		public void TestReopenTaskThrowsNullException()
		{
			//Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => GenerateRandomUser().ReopenTask(NullTask));
		}

		[TestMethod]
		public void TestReopenTaskThrowInvalidOperationException()
		{
			// Arrange
			var task = GenerateRandomTask();
			var creatorUser = task.CreatorUser;
			var targetUser = task.TargetUser;

			//Act and assert
			Assert.ThrowsException<InvalidOperationException>(() => creatorUser.ReopenTask(task));
			Assert.ThrowsException<InvalidOperationException>(() => targetUser.ReopenTask(task));
		}

		[TestMethod]
		public void TestReopenTaskThrowSecurityException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(user, ValidTaskDescription);
			user.FinishTask(task);

			//Act and assert
			Assert.ThrowsException<SecurityException>(() => GenerateRandomUser().ReopenTask(task));
		}

		public void TestReopenTaskOk()
		{
			// Act
			var creatorUser = GenerateRandomUser();
			var targetUser = GenerateRandomUser();

			var task1 = creatorUser.SetTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task1);
			targetUser.ReopenTask(task1);

			var task2 = creatorUser.SetTask(targetUser, ValidTaskDescription);
			targetUser.FinishTask(task2);
			creatorUser.ReopenTask(task2);

			var task3 = creatorUser.SetTask(targetUser, ValidTaskDescription);
			creatorUser.FinishTask(task3);
			targetUser.ReopenTask(task3);

			var task4 = creatorUser.SetTask(targetUser, ValidTaskDescription);
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