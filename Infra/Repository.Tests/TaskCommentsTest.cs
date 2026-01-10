using Domains.Exceptions;
using Repository.DTOs.Tasks;
using Repository.Tests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Repository.Tests
{
    public class TaskCommentsTest : RepositoryTestBase
    {
        [Fact]
        public async Task TryAddCommentWithNullUser_ThrowMissingArgumentException()
        {
            var data = new TaskCommentData
            {
                User = null,
                TaskId = Guid.NewGuid(),
                Comment = "Test"
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [Fact]
        public async Task TryAddCommentWithDefaultGuidAsTaskId_ThrowMissingArgumentException()
        {
            var data = new TaskCommentData();
            data.User = adminUser;
            data.TaskId = default;
            data.Comment = "Test";

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public async Task TryAddCommentWithInvalidComment_ThrowMissingArgumentException(string comment)
        {
            var data = new TaskCommentData();
            data.User = adminUser;
            data.TaskId = Guid.NewGuid();
            data.Comment = comment;

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [Fact]
        public async Task TryAddCommentWithNonRegistgeredUser_ThrowNotFoundException()
        {
            var data = new TaskCommentData()
            {
                Comment = "Test",
                User = GenerateRandomUser(),
                TaskId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await commentRepository.AddCommentAsync(data));
        }

        [Fact]
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

            Assert.Equal(result.Comment, comment.Text);
            Assert.Equal(result.UserId, comment.CreatedByUserId);
            Assert.NotEqual(result.CreatedAt, default);
        }
    }
}