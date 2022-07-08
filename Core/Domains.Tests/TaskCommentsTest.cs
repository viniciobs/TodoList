using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domains.Tests
{
    [TestClass]
    public class TaskCommentsTest : DomainTestBase
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void AddNullComment_ThrowMissingArgumentsException(string comment)
        {
            var task = normalUser.SelfAssignTask("Test");

            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.AddComment(task, comment));
        }

        [TestMethod]
        public void RandomUserTryAddComment_ThrowPermissionException()
        {
            var task = normalUser.SelfAssignTask("Test");
            var randomUser = GenerateRandomUser();

            Assert.ThrowsException<PermissionException>(() => randomUser.AddComment(task, "Test"));
        }

        [TestMethod]
        public void TryAddCommentToNullTask_ThrowMissingArgumentsException()
        {
            Assert.ThrowsException<MissingArgumentsException>(() => normalUser.AddComment(null, "Test"));
        }
    }
}