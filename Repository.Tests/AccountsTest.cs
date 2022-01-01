using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs.Accounts;
using Repository.Tests.Base;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class AccountsTest : RepositoryTestBase
	{
		[TestMethod]
		public void TestCreateAccountThrowDuplicateLoginException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var accountData = GetValidCreateAccountData();;
			
			repository.CreateAsync(accountData).Wait();
			repository.SaveChangesAsync().Wait();

			var resultException = repository.CreateAsync(accountData).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(RuleException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestCreateAccountThrowMissingArgumentsException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			CreateAccountData data = null;
			Exception resultException;

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new CreateAccountData();

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Login = EmptyLogin;
			data.Name = EmptyName;
			data.Password = EmptyPassword;

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data.Login = "  ";
			data.Name = "   ";
			data.Password = " ";

			resultException = repository.CreateAsync(data).Exception.InnerException;
			
			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new CreateAccountData()
			{
				Login = GenerateRandomString(),
				Name = GenerateRandomString()
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new CreateAccountData()
			{
				Name = GenerateRandomString(),
				Password = GenerateRandomString()
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			data = new CreateAccountData()
			{
				Login = GenerateRandomString(),
				Password = GenerateRandomString()
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestCreateAccountOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var data = GetValidCreateAccountData();

			var userId = repository.CreateAsync(data).Result;			
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNotNull(context.User.SingleOrDefault(x => x.Id == userId));

			context.Dispose();
		}

		[TestMethod]
		public void TestDeleteAccountThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var resultException = repository.DeleteAsync(Guid.NewGuid()).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestDeleteAccountOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var data = GetValidCreateAccountData();

			var userId = repository.CreateAsync(data).Result;
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNotNull(context.User.SingleOrDefault(x => x.Id == userId));

			// Act
			repository.DeleteAsync(userId).Wait();
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsNull(context.User.SingleOrDefault(x => x.Id == userId));

			context.Dispose();
		}

		[TestMethod]
		public void TestccountAuthenticateThrowMissingArgumentsException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			Exception resultException;
			AuthenticationData account = null;			

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			account = new AuthenticationData();

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			account.Login = NullLogin;
			account.Password = NullPassword;

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			account.Login = GenerateRandomString();
			account.Password = NullPassword;

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act
			account.Login = NullLogin;
			account.Password = GenerateRandomString();

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAuthenticateThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			AuthenticationData account = new AuthenticationData()
			{
				Login = GenerateRandomString(),
				Password = GenerateRandomString()
			};

			var resultException = repository.AuthenticateAsync(account).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAuthenticateOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createAccountData = GetValidCreateAccountData();
			
			repository.CreateAsync(createAccountData).Wait();
			repository.SaveChangesAsync().Wait();

			AuthenticationData authenticateData = new AuthenticationData()
			{
				Login = createAccountData.Login,
				Password = createAccountData.Password
			};

			var authenticationResult = repository.AuthenticateAsync(authenticateData).Result;

			// Assert
			Assert.IsNotNull(authenticationResult);
			
			Assert.IsTrue(authenticationResult.UserId != default);
			
			// At this point authenticationResult.Token is not setten yet.
			// Token must be validate in UI tests

			Assert.IsFalse(string.IsNullOrEmpty(authenticationResult.UserName.Trim()));

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAlterStatusThrowNotFoundException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var invalidUserId = Guid.NewGuid();
			var resultException = repository.AlterStatusAsync(invalidUserId, false).Exception.InnerException;

			// Assert
			Assert.AreEqual(resultException.GetType(), typeof(NotFoundException));

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAlterStatusOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);
		
			// Act
			var createAccountData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createAccountData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			// Assert
			Assert.IsTrue(user.IsActive);

			// Act
			repository.AlterStatusAsync(user.Id, false).Wait();
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsFalse(user.IsActive);

			// Act
			repository.AlterStatusAsync(user.Id, true).Wait();
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.IsTrue(user.IsActive);

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createAccountData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createAccountData).Result;
			repository.SaveChangesAsync().Wait();

			User user = context.User.Single(x => x.Id == userId);
			
			EditData editData = null;
			Exception resultException;

			resultException = repository.EditAsync(user, editData).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			// Act and assert
			try
			{
				editData = new EditData();

				repository.EditAsync(user, editData).Wait();
				
				editData.Login = EmptyLogin;
				editData.Name = EmptyName;

				repository.EditAsync(user, editData).Wait();

				editData.Login = "           ";
				editData.Name = "     ";

				repository.EditAsync(user, editData).Wait();
			}
			catch (Exception ex)
			{
				Assert.Fail("Expected no exception, but got: " + ex.Message);
				throw;
			}

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditThrowDuplicateLoginException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createUser1 = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createUser1).Result;
			repository.SaveChangesAsync().Wait();

			var createUser2 = GetValidCreateAccountData();

			var user2Id = repository.CreateAsync(createUser2).Result;
			repository.SaveChangesAsync().Wait();

			var editData = new EditData()
			{
				Login = createUser1.Login
			};

			var user2 = context.User.Single(x => x.Id == user2Id);

			var resultException = repository.EditAsync(user2, editData).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(RuleException), resultException.GetType());
			
			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditThrowUnauthorizeException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);								

			// Act
			var editData = new EditData()
			{
				Login = ValidLogin
			};

			var resultException = repository.EditAsync(null, editData).Exception.InnerException;

			// Assert
			Assert.AreEqual(typeof(UnauthorizeException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			var editData = new EditData()
			{
				Login = ValidLogin
			};

			// Assert
			Assert.AreNotEqual(user.Login, editData.Login);

			// Act
			repository.EditAsync(user, editData).Wait();
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(user.Login, editData.Login);

			// Act
			editData = new EditData()
			{
				Name = ValidName
			};

			// Assert
			Assert.AreNotEqual(user.Name, editData.Name);

			// Act
			repository.EditAsync(user, editData).Wait();
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(user.Name, editData.Name);

			context.Dispose();
		}


		[TestMethod]
		public void TestAccountChangePasswordThrowMissingArgumentException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = null;

			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			// Act
			data = new ChangePasswordData();
			
			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			// Act
			data.NewPassword = EmptyPassword;
			data.OldPassword = EmptyPassword;

			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			// Act
			data.NewPassword = "    ";
			data.OldPassword = "       ";

			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			// Act
			data.NewPassword = ValidPassword;
			data.OldPassword = "       ";

			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			// Act
			data.NewPassword = "    ";
			data.OldPassword = ValidPassword;

			// Assert
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));
		}

		[TestMethod]
		public void TestAccountChangePasswordThrowUnauthorizedException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();			

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = ValidPassword;
			data.OldPassword = createData.Password;

			// Assert
			Assert.ThrowsException<UnauthorizeException>(() => repository.ChangePassword(null, data));			
		}

		[TestMethod]
		public void TestAccountChangePasswordInvalidOldPasswordException()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = ValidPassword;
			data.OldPassword = GenerateRandomString();

			// Assert
			Assert.ThrowsException<RuleException>(() => repository.ChangePassword(user, data));
		}

		[TestMethod]
		public void TestAccountChangePasswordOk()
		{
			// Arrange
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			// Act
			var createData = GetValidCreateAccountData();

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = ValidPassword;
			data.OldPassword = createData.Password;

			// Assert
			Assert.AreNotEqual(user.Password, data.NewPassword);

			// Act
			repository.ChangePassword(user, data);
			repository.SaveChangesAsync().Wait();

			// Assert
			Assert.AreEqual(user.Password, data.NewPassword);
		}
	}
}
