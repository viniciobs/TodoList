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
            Assert.ThrowsExactly<MissingArgumentsException>(() => normalUser.SelfAssignTask(taskDescription));
        }

        [TestMethod]
        public void SetTaskToNullUser_ThrowMissingArgumentsException()
        {
            Assert.ThrowsExactly<MissingArgumentsException>(() => normalUser.AssignTask(null, "Test"));
        }

        [TestMethod]
        public void TryFinishFinishedTask_ThrowRuleException()
        {
            var task = normalUser.SelfAssignTask("Test");
            normalUser.FinishTask(task);

            Assert.ThrowsExactly<RuleException>(() => normalUser.FinishTask(task));
        }

        [TestMethod]
        public void TryFinishNullTask_TrhowMissingArgumentsException()
        {
            Assert.ThrowsExactly<MissingArgumentsException>(() => normalUser.FinishTask(null));
        }

        [TestMethod]
        public void NonTaskCreatorAndNonTaskTargetTryFinishTheTask_ThrowPermissionException()
        {
            var task = normalUser.SelfAssignTask("Test");

            Assert.ThrowsExactly<PermissionException>(() => adminUser.FinishTask(task));
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
            Assert.ThrowsExactly<MissingArgumentsException>(() => normalUser.ReopenTask(null));
        }

        [TestMethod]
        public void RandomUserTryReopenOthersTask_ThrowPermissionException()
        {
            var randomUser = GenerateRandomUser();

            var task = adminUser.AssignTask(normalUser, "Test");
            normalUser.FinishTask(task);

            Assert.ThrowsExactly<PermissionException>(() => randomUser.ReopenTask(task));
        }

        [TestMethod]
        public void TryReopenUnfinishedTask_ThrowRuleException()
        {
            var task = normalUser.SelfAssignTask("Test");

            Assert.ThrowsExactly<RuleException>(() => normalUser.ReopenTask(task));
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