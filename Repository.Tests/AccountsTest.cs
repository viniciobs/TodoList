using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs.Accounts;
using Repository.Tests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Tests
{
    [TestClass]
    public class AccountsTest : RepositoryTestBase
    {
        [TestMethod]
        public async Task CreateAccountWithExistingLogin_ThrowRuleException()
        {
            var accountData = GenerateValidCreateAccountData(); ;

            await repository.CreateAsync(accountData);
            await repository.SaveChangesAsync();

            await Assert.ThrowsExceptionAsync<RuleException>(async () => await repository.CreateAsync(accountData));
        }

        [TestMethod]
        public async Task CreateAccount_Ok()
        {
            var newAccountData = GenerateValidCreateAccountData();

            var userId = await repository.CreateAsync(newAccountData);

            await repository.SaveChangesAsync();

            Assert.IsNotNull(context.User.SingleOrDefault(x => x.Id == userId));
        }

        [TestMethod]
        public async Task TryDeleteUnexistentAccount_ThrowNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await repository.DeleteAsync(Guid.NewGuid()));
        }

        [TestMethod]
        public async Task DeleteAccount_Ok()
        {
            var newAccountData = GenerateValidCreateAccountData();

            var userId = await repository.CreateAsync(newAccountData);
            await repository.SaveChangesAsync();

            await repository.DeleteAsync(userId);
            await repository.SaveChangesAsync();

            Assert.IsFalse(context.User.Any(x => x.Id == userId));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task AuthenticateWithInvalidLogin_ThrowMissingArgumentsException(string login)
        {
            var authenticationData = new AuthenticationData()
            {
                Login = login,
                Password = "1234"
            };

            await Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await repository.AuthenticateAsync(authenticationData));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task AuthenticateWithInvalidPassword_ThrowMissingArgumentsException(string password)
        {
            var authenticationData = new AuthenticationData()
            {
                Login = "test",
                Password = password
            };

            await Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await repository.AuthenticateAsync(authenticationData));
        }

        [TestMethod]
        public async Task TryToAuthenticateWithInvalidAccount_ThrowNotFoundException()
        {
            AuthenticationData account = new()
            {
                Login = "this_l0g1n_does_not_exist_F0r_SuR3",
                Password = "1234"
            };

            await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await repository.AuthenticateAsync(account));
        }

        [TestMethod]
        public async Task AccountAuthenticate_Ok()
        {
            var createAccountData = GenerateValidCreateAccountData();

            var newAccountId = await repository.CreateAsync(createAccountData);
            await repository.SaveChangesAsync();

            AuthenticationData authenticateData = new()
            {
                Login = createAccountData.Login,
                Password = createAccountData.Password
            };

            var authenticationResult = await repository.AuthenticateAsync(authenticateData);

            Assert.IsNotNull(authenticationResult);
            Assert.IsTrue(authenticationResult.UserId == newAccountId);

            // At this point authenticationResult.Token is not setten yet.
            // Token must be validate in UI tests

            Assert.IsFalse(string.IsNullOrEmpty(authenticationResult.Login.Trim()));
        }

        [TestMethod]
        public async Task TryAlterStatusOfUnexistentAccount_ThrowNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await repository.AlterStatusAsync(Guid.NewGuid(), false));
        }

        [TestMethod]
        public async Task AccountEditWithExistentLogin_ThrowRuleException()
        {
            var createUser1 = GenerateValidCreateAccountData();
            var userId = await repository.CreateAsync(createUser1);
            await repository.SaveChangesAsync();

            var createUser2 = GenerateValidCreateAccountData();
            var user2Id = await repository.CreateAsync(createUser2);
            await repository.SaveChangesAsync();

            var editData = new EditData()
            {
                Login = createUser1.Login
            };

            var user2 = context.User.Single(x => x.Id == user2Id);

            await Assert.ThrowsExceptionAsync<RuleException>(async () => await repository.EditAsync(user2, editData));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ChangePasswordToInvalidOne_ThrowMissingArgumentException(string newPassword)
        {
            var createData = GenerateValidCreateAccountData();

            var userId = await repository.CreateAsync(createData);
            await repository.SaveChangesAsync();

            var user = context.User.Single(x => x.Id == userId);

            ChangePasswordData data = new()
            {
                OldPassword = "1234",
                NewPassword = newPassword
            };

            Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ChangePasswordSendingInvalidOldPassword_ThrowMissingArgumentException(string oldPassword)
        {
            var createData = GenerateValidCreateAccountData();

            var userId = await repository.CreateAsync(createData);
            await repository.SaveChangesAsync();

            var user = context.User.Single(x => x.Id == userId);

            ChangePasswordData data = new()
            {
                OldPassword = oldPassword,
                NewPassword = "1234"
            };

            Assert.ThrowsException<MissingArgumentsException>(() => repository.ChangePassword(user, data));
        }

        [TestMethod]
        public async Task ChangePasswordWithWrongOldPassword_ThrowRuleException()
        {
            var createData = GenerateValidCreateAccountData();

            var userId = repository.CreateAsync(createData).Result;
            await repository.SaveChangesAsync();

            var user = context.User.Single(x => x.Id == userId);

            ChangePasswordData data = new();
            data.NewPassword = "test";
            data.OldPassword = "this_p4ssw0rd_does_not_Exist_f0R_Sur3";

            Assert.ThrowsException<RuleException>(() => repository.ChangePassword(user, data));
        }
    }
}