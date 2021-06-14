﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using Domains.Tests;
using System.Linq;

namespace Tests
{
	[TestClass]
	public class TaskCommentsTest : TestBase
	{
		[TestMethod]
		public void TestAddTaskThrowNullException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(GenerateRandomUser(), ValidTaskDescription);

			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => user.AddComment(NullTask, ValidComment));
			Assert.ThrowsException<ArgumentNullException>(() => user.AddComment(task, NullComment));
			Assert.ThrowsException<ArgumentNullException>(() => user.AddComment(task, EmptyComment));
		}

		[TestMethod]
		public void TestAddCommentThrowSecurityException()
		{
			// Act and assert
			Assert.ThrowsException<SecurityException>(() => GenerateRandomUser().AddComment(GenerateRandomTask(), ValidComment));
		}

		[TestMethod]
		public void TestAddCommentThrowNullOrEmptyException()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(GenerateRandomUser(), ValidTaskDescription);

			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => user.AddComment(task, NullComment));
			Assert.ThrowsException<ArgumentNullException>(() => user.AddComment(task, EmptyComment));
		}

		[TestMethod]
		public void TestAddCommentOk()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(GenerateRandomUser(), ValidTaskDescription);

			user.AddComment(task, ValidComment);

			// Act and assert
			Assert.AreEqual(ValidComment, task.Comments.Single().Text);
		}

		[TestMethod]
		public void TestAddMultipleCommentOk()
		{
			// Arrange
			var user = GenerateRandomUser();
			var task = user.SetTask(user, ValidTaskDescription);
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