using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domains.Tests
{
    [TestClass]
    public class TasksTest : DomainTestBase
    {
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("   ")]
        public void SetTaskWithInvalidDescription_ThrowsMissingArgumentsException(string taskDescription)
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.SelfAssignTask(taskDescription));
        }

        [TestMethod]
        public void SetTaskToNullUser_ThrowMissingArgumentsException()
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.AssignTask(null, "Test"));
        }

        [TestMethod]
        public void TryFinishFinishedTask_ThrowRuleException()
        {
            var task = normalUser.SelfAssignTask(ValidTaskDescription);
            normalUser.FinishTask(task);

            Assert.ThrowsException<RuleException>(() => normalUser.FinishTask(task));
        }

        [TestMethod]
        public void TryFinishNullTask_TrhowMissingArgumentsException()
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.FinishTask(null));
        }

        [TestMethod]
        public void NonTaskCreatorAndNonTaskTargetTryFinishTheTask_ThrowPermissionException()
        {
            var task = normalUser.SelfAssignTask("Test");

            Assert.ThrowsException<PermissionException>(() => adminUser.FinishTask(task));
        }

        [TestMethod]
        public void TaskCreatorFinishTask_OK()
        {
            var task = normalUser.SelfAssignTask("Test");
            normalUser.FinishTask(task);

            Assert.IsNotNull(task.CompletedAt);
        }

        [TestMethod]
        public void TaskTargetFinishTask_Ok()
        {
            var task = adminUser.AssignTask(normalUser, "Test");
            normalUser.FinishTask(task);

            Assert.IsNotNull(task.CompletedAt);
        }

        [TestMethod]
        public void ReopenNullTask_ThrowsMissingArgumentsException()
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.ReopenTask(null));
        }

        [TestMethod]
        public void RandomUserTryReopenOthersTask_ThrowPermissionException()
        {
            var randomUser = GenerateRandomUser();

            var task = adminUser.AssignTask(normalUser, "Test");
            normalUser.FinishTask(task);

            Assert.ThrowsException<PermissionException>(() => randomUser.ReopenTask(task));
        }

        [TestMethod]
        public void TryReopenUnfinishedTask_ThrowRuleException()
        {
            var task = normalUser.SelfAssignTask("Test");

            Assert.ThrowsException<RuleException>(() => normalUser.ReopenTask(task));
        }

        [TestMethod]
        public void TaskCreatorReopenFinishedTask_Ok()
        {
            var task = adminUser.AssignTask(normalUser, "Test");
            normalUser.FinishTask(task);

            adminUser.ReopenTask(task);

            Assert.IsNull(task.CompletedAt);
        }

        [TestMethod]
        public void TaskTargetReopenFinishedTask_Ok()
        {
            var task = adminUser.AssignTask(normalUser, "Test");
            normalUser.FinishTask(task);

            normalUser.ReopenTask(task);

            Assert.IsNull(task.CompletedAt);
        }
    }
}