using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domains.Tests
{
    [TestClass]
    public class UsersTest : DomainTestBase
    {
        [TestMethod]
        [DataRow("  ", null)]
        [DataRow("some name", "")]
        public void CreateUserWithInvalidArguments_ThrowMissingArgumentsException(string name, string login)
        {
            Assert.ThrowsException<MissingArgumentsException>(() => User.New(name, login));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void SetInvalidPassword_ThrowMissingArgumentsException(string password)
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.SetPassword(password));
        }

        [TestMethod]
        public void DeactivateWithOpenedTask_ThrowRuleException()
        {
            normalUser.SelfAssignTask("Test");

            Assert.ThrowsException<RuleException>(() => normalUser.Deactivate());
        }

        [TestMethod]
        public void DeactivateWithAllTasksFinished_Ok()
        {
            var randomUser = GenerateRandomUser();

            var task = randomUser.SelfAssignTask("Test");

            randomUser.FinishTask(task);
            randomUser.Deactivate();

            Assert.IsFalse(randomUser.IsActive);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void SetInvalidName_ThrowMissingArgumentException(string newName)
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.SetName(newName));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void SetInvalidLogin_ThrowMissingArgumentException(string newLogin)
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.SetLogin(newLogin));
        }

        [TestMethod]
        public void NormalUserSetSelfRole_ThrowPermissionException()
        {
            Assert.ThrowsException<PermissionException>(() => normalUser.AlterUserRole(normalUser, UserRole.Admin));
        }

        [TestMethod]
        public void SetRoleToNullUser_ThrowMissingArgumentsException()
        {
            Assert.ThrowsException<MissingArgumentsException>(() => GenerateAdminUser().AlterUserRole(null, UserRole.Admin));
        }

        [TestMethod]
        public void AlterOwnRole_ThrowRuleException()
        {
            Assert.ThrowsException<RuleException>(() => adminUser.AlterUserRole(adminUser, UserRole.Normal));
        }

        [TestMethod]
        public void TryChangeOtherUserRoleWithTheAlreadyGivenRole_ThrowRuleException()
        {
            Assert.ThrowsException<RuleException>(() => adminUser.AlterUserRole(normalUser, UserRole.Normal));
        }
    }
}