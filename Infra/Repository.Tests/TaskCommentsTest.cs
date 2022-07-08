using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs.Tasks;
using Repository.Tests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Tests
{
    [TestClass]
    public class TaskCommentsTest : RepositoryTestBase
    {
        [TestMethod]
        public void TryAddCommentWithNullUser_ThrowMissingArgumentException()
        {
            var data = new TaskCommentData();
            data.User = null;
            data.TaskId = Guid.NewGuid();
            data.Comment = "Test";

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [TestMethod]
        public void TryAddCommentWithDefaultGuidAsTaskId_ThrowMissingArgumentException()
        {
            var data = new TaskCommentData();
            data.User = adminUser;
            data.TaskId = default;
            data.Comment = "Test";

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("     ")]
        [DataRow(null)]
        public void TryAddCommentWithInvalidComment_ThrowMissingArgumentException(string comment)
        {
            var data = new TaskCommentData();
            data.User = adminUser;
            data.TaskId = Guid.NewGuid();
            data.Comment = comment;

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [TestMethod]
        public void TryAddCommentWithNonRegistgeredUser_ThrowNotFoundException()
        {
            var data = new TaskCommentData()
            {
                Comment = "Test",
                User = GenerateRandomUser(),
                TaskId = Guid.NewGuid()
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [TestMethod]
        public async Task AddComment_Ok()
        {
            EnsureUserIsActive(adminUser);
            EnsureUserIsActive(normalUser);

            var assingTaskData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                TargetUser = normalUser,
                Description = "Test"
            };

            var task = await taskRepository.AssignAsync(assingTaskData);
            await taskRepository.SaveChangesAsync();

            var data = new TaskCommentData()
            {
                Comment = "Test",
                User = adminUser,
                TaskId = task.Id
            };

            var result = await commentRepository.AddCommentAsync(data);
            await commentRepository.SaveChangesAsync();

            var comment = context.TaskComment.Single();

            Assert.AreEqual(result.Comment, comment.Text);
            Assert.AreEqual(result.UserId, comment.CreatedByUserId);
            Assert.AreNotEqual(result.CreatedAt, default);
        }
    }
}