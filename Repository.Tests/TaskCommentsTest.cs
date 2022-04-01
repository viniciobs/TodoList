using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository._Commom;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Tests.Base;
using Repository.Tests.Seed;
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

        //[TestMethod]
        //public void TestGetByFilterOk()
        //{
        //    // Act
        //    adminUser = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;
        //    var anotherRandomUserId = accountRepository.CreateAsync(GenerateValidCreateAccountData()).Result;

        //    accountRepository.SaveChangesAsync().Wait();

        //    var randomUser = context.User.Single(x => x.Id == adminUser);
        //    var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);

        //    var randomTask = taskRepository.AssignAsync(new AssignTaskData()
        //    {
        //        CreatorUser = anotherRandomUser,
        //        TargetUser = randomUser,
        //        Description = ValidTaskDescription
        //    }).Result;

        //    var anotherRandomTask = taskRepository.AssignAsync(new AssignTaskData()
        //    {
        //        CreatorUser = anotherRandomUser,
        //        TargetUser = randomUser,
        //        Description = ValidTaskDescription
        //    }).Result;

        //    taskRepository.SaveChangesAsync().Wait();

        //    var data = new TaskCommentData()
        //    {
        //        Comment = ValidComment,
        //        User = randomUser,
        //        TaskId = randomTask.Id
        //    };

        //    taskCommentRepository.AddCommentAsync(data).Wait();

        //    data = new TaskCommentData()
        //    {
        //        Comment = GenerateRandomString(),
        //        User = anotherRandomUser,
        //        TaskId = randomTask.Id
        //    };

        //    taskCommentRepository.AddCommentAsync(data).Wait();

        //    data = new TaskCommentData()
        //    {
        //        Comment = GenerateRandomString(),
        //        User = randomUser,
        //        TaskId = anotherRandomTask.Id
        //    };

        //    taskCommentRepository.AddCommentAsync(data).Wait();
        //    taskCommentRepository.SaveChangesAsync().Wait();

        //    PaginationResult<TaskCommentResult> result;
        //    TaskCommentFilter filter = null;

        //    result = taskCommentRepository.GetAsync(filter).Result;

        //    // Assert
        //    Assert.AreEqual(result.Data.Count(), 3);

        //    // Act
        //    filter = new TaskCommentFilter()
        //    {
        //        TaskId = randomTask.Id
        //    };

        //    result = taskCommentRepository.GetAsync(filter).Result;

        //    // Assert
        //    Assert.AreEqual(result.Data.Count(), 2);

        //    // Act
        //    filter = new TaskCommentFilter()
        //    {
        //        TaskId = anotherRandomTask.Id
        //    };

        //    result = taskCommentRepository.GetAsync(filter).Result;

        //    // Assert
        //    Assert.AreEqual(result.Data.Count(), 1);

        //    // Act
        //    var date = DateTime.Today.AddDays(-2);

        //    var comment1 = context.TaskComment.First();
        //    comment1.AlterCreatedAt(date);

        //    var comment2 = context.TaskComment.Last();
        //    comment2.AlterCreatedAt(date);

        //    context.TaskComment.UpdateRange(comment1, comment2);
        //    context.SaveChanges();

        //    filter = new TaskCommentFilter()
        //    {
        //        CreatedBetween = new Period(date.AddHours(-1), date.AddHours(1))
        //    };

        //    result = taskCommentRepository.GetAsync(filter).Result;

        //    // Assert
        //    Assert.AreEqual(result.Data.Count(), 2);

        //    // Act
        //    filter = new TaskCommentFilter()
        //    {
        //        CreatedBetween = new Period(DateTime.Today, null)
        //    };

        //    result = taskCommentRepository.GetAsync(filter).Result;

        //    // Assert
        //    Assert.AreEqual(result.Data.Count(), 1);

        //    context.Dispose();
        //}
    }
}