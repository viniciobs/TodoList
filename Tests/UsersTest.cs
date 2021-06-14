using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domains;
using System;
using Domains.Tests;

namespace Tests
{
	[TestClass]
	public class UsersTest : TestBase
	{
		[TestMethod]
		public void TestUserCreationThrowExceptionNullAndEmptyArguments()
		{
			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => User.New(NullName, NullLogin));
			Assert.ThrowsException<ArgumentNullException>(() => User.New(NullName, EmptyLogin));
			Assert.ThrowsException<ArgumentNullException>(() => User.New(NullName, ValidLogin));

			Assert.ThrowsException<ArgumentNullException>(() => User.New(EmptyName, NullLogin));
			Assert.ThrowsException<ArgumentNullException>(() => User.New(EmptyName, EmptyLogin));
			Assert.ThrowsException<ArgumentNullException>(() => User.New(EmptyName, ValidLogin));

			Assert.ThrowsException<ArgumentNullException>(() => User.New(ValidName, NullLogin));
			Assert.ThrowsException<ArgumentNullException>(() => User.New(ValidName, EmptyLogin));
		}

		[TestMethod]
		public void TestUserCreationOk()
		{
			// Act
			var user = User.New(ValidName, ValidLogin);

			// Assert
			Assert.IsInstanceOfType(user, typeof(User));
		}

		[TestMethod]
		public void TestUserSetPasswordThrowExceptionNullAndEmptyArguments()
		{
			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => GenerateRandomUser().SetPassword(NullPassword));
			Assert.ThrowsException<ArgumentNullException>(() => GenerateRandomUser().SetPassword(EmptyPassword));
		}

		[TestMethod]
		public void TestUserSetPasswordOk()
		{
			// Act
			var randomUser = GenerateRandomUser();
			randomUser.SetPassword(ValidPassword);

			// Assert
			Assert.AreEqual(ValidPassword, randomUser.Password);
		}

		[TestMethod]
		public void TestUserSetRoleThrowInvalidOperationException()
		{
			// Act
			var randomUser = GenerateRandomUser();

			// Assert
			Assert.ThrowsException<InvalidOperationException>(() => randomUser.AlterUserRole(GenerateRandomUser(), UserRole.Admin));
			Assert.ThrowsException<InvalidOperationException>(() => randomUser.AlterUserRole(randomUser, UserRole.Admin));
		}

		[TestMethod]
		public void TestUserSetRoleThrowArgumentNullException()
		{
			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => GenerateAdminUser().AlterUserRole(NullUser, UserRole.Admin));
		}

		[TestMethod]
		public void TestUserSetRoleThrowApplicationException()
		{
			// Act
			var adminUser = GenerateAdminUser();
			var randomUser = GenerateRandomUser();

			var adminUser2 = GenerateAdminUser();

			// Assert
			Assert.ThrowsException<ApplicationException>(() => adminUser.AlterUserRole(randomUser, UserRole.Normal));
			Assert.ThrowsException<ApplicationException>(() => adminUser2.AlterUserRole(adminUser, UserRole.Admin));
		}

		[TestMethod]
		public void TestUserSetRoleOk()
		{
			// Act
			var adminUser = GenerateAdminUser();
			var randomUser = GenerateRandomUser();

			adminUser.AlterUserRole(randomUser, UserRole.Admin);

			// Assert
			Assert.AreEqual(randomUser.Role, UserRole.Admin);

			// Act 2
			adminUser.AlterUserRole(randomUser, UserRole.Normal);

			// Assert 2
			Assert.AreEqual(randomUser.Role, UserRole.Normal);
		}
	}
}