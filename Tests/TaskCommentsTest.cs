using Domains.Exceptions;
using Domains.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security;

namespace Tests
{
	[TestClass]
	public class TaskCommentsTest : TestBase
	{
		[TestMethod]
		public void TestAddTaskThrowMissingArgumentsException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(GenerateRandomUser(), ValidTaskDescription);

			// Act and assert
			Assert.ThrowsException<MissingArgumentsException>(() => user.AddComment(NullTask, ValidComment));
			Assert.ThrowsException<MissingArgumentsException>(() => user.AddComment(task, NullComment));
			Assert.ThrowsException<MissingArgumentsException>(() => user.AddComment(task, EmptyComment));
		}

		[TestMethod]
		public void TestAddCommentThrowPermissionException()
		{
			// Act and assert
			Assert.ThrowsException<PermissionException>(() => GenerateRandomUser().AddComment(GenerateRandomTask(), ValidComment));
		}

		[TestMethod]
		public void TestAddCommentThrowMissingArgumentsException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(GenerateRandomUser(), ValidTaskDescription);

			// Act and assert
			Assert.ThrowsException<MissingArgumentsException>(() => user.AddComment(task, NullComment));
			Assert.ThrowsException<MissingArgumentsException>(() => user.AddComment(task, EmptyComment));
		}

		[TestMethod]
		public void TestAddCommentOk()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(GenerateRandomUser(), ValidTaskDescription);

			user.AddComment(task, ValidComment);

			// Act and assert
			Assert.AreEqual(ValidComment, task.Comments.Single().Text);
		}

		[TestMethod]
		public void TestAddMultipleCommentOk()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.AssignTask(user, ValidTaskDescription);
			var comment1 = GenerateRandomString();
			var comment2 = GenerateRandomString();
			var comment3 = GenerateRandomString();

			user.AddComment(task, comment1);
			user.AddComment(task, comment2);
			user.AddComment(task, comment3);

			// Act and assert
			Assert.AreEqual(comment1, task.Comments.Where((x, i) => i == 0).Single().Text);
			Assert.AreEqual(comment2, task.Comments.Where((x, i) => i == 1).Single().Text);
			Assert.AreEqual(comment3, task.Comments.Where((x, i) => i == 2).Single().Text);
			Assert.AreEqual(3, task.Comments.Count);
		}
	}
}