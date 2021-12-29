using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs.Accounts;
using Repository.Tests.Seed;
using System;
using System.Linq;

namespace Repository.Tests
{
	[TestClass]
	public class AccountsTest
	{
		[TestMethod]
		public void TestCreateAccountThrowDuplicateLoginException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var user1 = new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			repository.CreateAsync(user1).Wait();
			repository.SaveChangesAsync().Wait();

			var user2 = new CreateAccountData()
			{
				Login = "princeOfDarkness",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var resultException = repository.CreateAsync(user2).Exception.InnerException;

			Assert.AreEqual(typeof(RuleException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestCreateAccountThrowMissingArgumentsException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			CreateAccountData data = null;
			Exception resultException;

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new CreateAccountData();

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data.Login = "";
			data.Name = "";
			data.Password = "";

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data.Login = "  ";
			data.Name = "   ";
			data.Password = " ";

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne"
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new CreateAccountData()
			{
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			data = new CreateAccountData()
			{
				Login = "ozzy",
				Password = "1234"
			};

			resultException = repository.CreateAsync(data).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestCreateAccountOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var data = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(data).Result;			
			repository.SaveChangesAsync().Wait();

			Assert.IsNotNull(context.User.SingleOrDefault(x => x.Id == userId));

			context.Dispose();
		}

		[TestMethod]
		public void TestDeleteAccountThrowNotFoundException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var resultException = repository.DeleteAsync(Guid.NewGuid()).Exception.InnerException;

			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestDeleteAccountOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var data = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(data).Result;
			repository.SaveChangesAsync().Wait();

			Assert.IsNotNull(context.User.SingleOrDefault(x => x.Id == userId));

			repository.DeleteAsync(userId).Wait();
			repository.SaveChangesAsync().Wait();

			Assert.IsNull(context.User.SingleOrDefault(x => x.Id == userId));

			context.Dispose();
		}

		[TestMethod]
		public void TestccountAuthenticateThrowMissingArgumentsException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			Exception resultException;
			AuthenticationData account = null;			

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			account = new AuthenticationData();

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			account.Login = "";
			account.Password = "";

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			account.Login = "ozzy";
			account.Password = null;

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			account.Login = null;
			account.Password = "1234";

			resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAuthenticateThrowNotFoundException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			AuthenticationData account = new AuthenticationData()
			{
				Login = "ozzy",
				Password = "1234"
			};

			var resultException = repository.AuthenticateAsync(account).Exception.InnerException;
			Assert.AreEqual(typeof(NotFoundException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAuthenticateOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createAccountData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};
			
			repository.CreateAsync(createAccountData).Wait();
			repository.SaveChangesAsync().Wait();

			AuthenticationData authenticateData = new AuthenticationData()
			{
				Login = "ozzy",
				Password = "1234"
			};

			var authenticationResult = repository.AuthenticateAsync(authenticateData).Result;

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
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var invalidUserId = Guid.NewGuid();
			var resultException = repository.AlterStatusAsync(invalidUserId, false).Exception.InnerException;

			Assert.AreEqual(resultException.GetType(), typeof(NotFoundException));

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountAlterStatusOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);
		
			var createAccountData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createAccountData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			Assert.IsTrue(user.IsActive);

			repository.AlterStatusAsync(user.Id, false).Wait();
			repository.SaveChangesAsync().Wait();

			Assert.IsFalse(user.IsActive);

			repository.AlterStatusAsync(user.Id, true).Wait();
			repository.SaveChangesAsync().Wait();

			Assert.IsTrue(user.IsActive);

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditThrowMissingArgumentException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createAccountData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createAccountData).Result;
			repository.SaveChangesAsync().Wait();

			User user = context.User.Single(x => x.Id == userId);
			
			EditData editData = null;
			Exception resultException;

			resultException = repository.EditAsync(user, editData).Exception.InnerException;
			Assert.AreEqual(typeof(MissingArgumentsException), resultException.GetType());

			try
			{
				editData = new EditData();

				repository.EditAsync(user, editData).Wait();
				
				editData.Login = "";
				editData.Name = "";

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
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createUser1 = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createUser1).Result;
			repository.SaveChangesAsync().Wait();

			var createUser2 = new CreateAccountData()
			{
				Login = "zakk",
				Name = "Zakk Wylde",
				Password = "1234"
			};

			var user2Id = repository.CreateAsync(createUser2).Result;
			repository.SaveChangesAsync().Wait();

			var editData = new EditData()
			{
				Login = "ozzy"
			};

			var user2 = context.User.Single(x => x.Id == user2Id);

			var resultException = repository.EditAsync(user2, editData).Exception.InnerException;
			Assert.AreEqual(typeof(RuleException), resultException.GetType());
			
			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditThrowUnauthorizeException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			repository.CreateAsync(createData).Wait();
			repository.SaveChangesAsync().Wait();			

			var editData = new EditData()
			{
				Login = "princeOfDarkness"
			};

			var resultException = repository.EditAsync(null, editData).Exception.InnerException;
			Assert.AreEqual(typeof(UnauthorizeException), resultException.GetType());

			context.Dispose();
		}

		[TestMethod]
		public void TestAccountEditOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			var editData = new EditData()
			{
				Login = "princeOfDarkness"
			};

			Assert.AreNotEqual(user.Login, editData.Login);

			repository.EditAsync(user, editData).Wait();
			repository.SaveChangesAsync().Wait();

			Assert.AreEqual(user.Login, editData.Login);

			editData = new EditData()
			{
				Name = "John Michael Osbourne"
			};

			Assert.AreNotEqual(user.Name, editData.Name);

			repository.EditAsync(user, editData).Wait();
			repository.SaveChangesAsync().Wait();

			Assert.AreEqual(user.Name, editData.Name);

			context.Dispose();
		}


		[TestMethod]
		public void TestAccountChangePasswordThrowMissingArgumentException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = null;

			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			data = new ChangePasswordData();
			
			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			data.NewPassword = "";
			data.OldPassword = "";

			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			data.NewPassword = "    ";
			data.OldPassword = "       ";

			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			data.NewPassword = "1234";
			data.OldPassword = "       ";

			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));

			data.NewPassword = "    ";
			data.OldPassword ="1234";

			Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));
		}

		[TestMethod]
		public void TestAccountChangePasswordThrowUnauthorizedException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();			

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = "123456";
			data.OldPassword = "1234";

			Assert.ThrowsException<UnauthorizeException>(() => repository.ChangePassword(null, data));			
		}

		[TestMethod]
		public void TestAccountChangePasswordInvalidOldPasswordException()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = "123456789";
			data.OldPassword = "123456";

			Assert.ThrowsException<RuleException>(() => repository.ChangePassword(user, data));
		}

		[TestMethod]
		public void TestAccountChangePasswordOk()
		{
			var context = new FakeContext().DbContext;
			var repository = new AccountRepository(context);

			var createData = new CreateAccountData()
			{
				Login = "ozzy",
				Name = "Ozzy Osbourne",
				Password = "1234"
			};

			var userId = repository.CreateAsync(createData).Result;
			repository.SaveChangesAsync().Wait();

			var user = context.User.Single(x => x.Id == userId);

			ChangePasswordData data = new ChangePasswordData();
			data.NewPassword = "123456789";
			data.OldPassword = "1234";

			Assert.AreNotEqual(user.Password, data.NewPassword);

			repository.ChangePassword(user, data);
			repository.SaveChangesAsync().Wait();

			Assert.AreEqual(user.Password, data.NewPassword);
		}
	}
}
