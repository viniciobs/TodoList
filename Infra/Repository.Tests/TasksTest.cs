using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs._Commom;
using Repository.DTOs.Tasks;
using Repository.Pagination;
using Repository.Tests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Tests
{
    [TestClass]
    public class TasksTest : RepositoryTestBase
    {
        [TestMethod]
        public async Task AssignToNullTargetUser_ThrowMissingArgumentsException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                Description = "Test",
                TargetUser = null
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public async Task AssignToNullCreatorUser_ThrowMissingArgumentsException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = null,
                Description = "Test",
                TargetUser = adminUser
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task AssignWithNUllDescription_ThrowMissingArgumentsException(string description)
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                Description = description,
                TargetUser = normalUser
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public void AssignToInactiveTargetUser_ThrowRuleException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                TargetUser = GenerateDeactivatedUser(),
                Description = "Test"
            };

            Assert.ThrowsExceptionAsync<RuleException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public void AssignToInactiveCreatorUser_ThrowRuleException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = GenerateDeactivatedUser(),
                TargetUser = adminUser,
                Description = "Test"
            };

            Assert.ThrowsExceptionAsync<RuleException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public async Task Assign_EnsureCreatedDateIsNull()
        {
            var data = new AssignTaskData()
            {
                CreatorUser = adminUser,
                TargetUser = normalUser,
                Description = "Test"
            };

            EnsureUserIsActive(adminUser);
            EnsureUserIsActive(normalUser);

            var taskResult = await taskRepository.AssignAsync(data);
            await taskRepository.SaveChangesAsync();

            var task = context.Task.Single(x => x.Id == taskResult.Id);

            Assert.IsNull(task.CompletedAt);
        }

        [TestMethod]
        public async Task TryAssignToNullTargetUser_ThrowNotFoundException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = normalUser,
                TargetUser = null,
                Description = "Test"
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public async Task TryAssignToNullCreatorUser_ThrowNotFoundException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = null,
                TargetUser = normalUser,
                Description = "Test"
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [TestMethod]
        public async Task TryFinishTaskWithNullUser_ThrowMissingArgumentException()
        {
            EnsureUserIsActive(adminUser);

            var taskData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                TargetUser = normalUser,
                Description = "Test"
            };

            var task = await taskRepository.AssignAsync(taskData);
            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                TaskId = task.Id,
                User = null
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.FinishAsync(data));
        }

        [TestMethod]
        public async Task TryFinishTaskWithInvalidTaskId_ThrowMissingArgumentException()
        {
            var data = new UserTask()
            {
                TaskId = default,
                User = normalUser
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.FinishAsync(data));
        }

        [TestMethod]
        public void TryFinishTaskWithInvalidTaskId_ThrowNotFoundException()
        {
            var data = new UserTask()
            {
                User = adminUser,
                TaskId = Guid.NewGuid()
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.FinishAsync(data));
        }

        [TestMethod]
        public async Task FinishTask_EnsureCompletedDateIsNotNull()
        {
            EnsureUserIsActive(normalUser);

            var assignTask = new AssignTaskData()
            {
                CreatorUser = normalUser,
                TargetUser = normalUser,
                Description = "Test"
            };

            var taskToFinish = await taskRepository.AssignAsync(assignTask);
            await taskRepository.SaveChangesAsync();

            var finishData = new UserTask()
            {
                TaskId = taskToFinish.Id,
                User = normalUser
            };

            await taskRepository.FinishAsync(finishData);
            await taskRepository.SaveChangesAsync();

            var finishedTask = context.Task.Single(x => x.Id == taskToFinish.Id);

            Assert.IsNotNull(finishedTask.CompletedAt);
        }

        [TestMethod]
        public async Task TryReopenWithNullUser_ThrowMissingArgumentException()
        {
            var taskToFinish = normalUser.SelfAssignTask("Test");
            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                User = null,
                TaskId = taskToFinish.Id
            };

            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await taskRepository.ReopenAsync(data));
        }

        [TestMethod]
        public async Task TryReopenWithInvalidUser_ThrowNotFoundException()
        {
            var taskToFinish = normalUser.SelfAssignTask("Test");
            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                User = GenerateRandomUser(),
                TaskId = taskToFinish.Id
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await taskRepository.ReopenAsync(data));
        }

        [TestMethod]
        public async Task ReopenTask_EnsureCompletedDateWasNotNullAndAfterReopenedIsNull()
        {
            EnsureUserIsActive(adminUser);
            EnsureUserIsActive(normalUser);

            var task = normalUser.SelfAssignTask("Test");
            context.Entry(task).State = EntityState.Added;

            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                User = normalUser,
                TaskId = task.Id
            };

            await taskRepository.FinishAsync(data);
            await taskRepository.SaveChangesAsync();

            task = context.Task.Single(x => x.Id == task.Id);

            Assert.IsNotNull(task.CompletedAt);

            await taskRepository.ReopenAsync(data);
            await taskRepository.SaveChangesAsync();

            task = context.Task.Single(x => x.Id == task.Id);

            Assert.IsNull(task.CompletedAt);
        }

        [TestMethod]
        public async Task TestGetByFilterOk()
        {
            var randomUserId = await accountRepository.CreateAsync(GenerateValidCreateAccountData());
            var anotherRandomUserId = await accountRepository.CreateAsync(GenerateValidCreateAccountData());
            var oneMoreRandomUserId = await accountRepository.CreateAsync(GenerateValidCreateAccountData());

            await accountRepository.SaveChangesAsync();

            var randomUser = context.User.Single(x => x.Id == randomUserId);
            var anotherRandomUser = context.User.Single(x => x.Id == anotherRandomUserId);
            var oneMoreRandomUser = context.User.Single(x => x.Id == oneMoreRandomUserId);

            await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = randomUser,
                TargetUser = anotherRandomUser,
                Description = GenerateRandomString()
            });

            var someTask = taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = randomUser,
                TargetUser = oneMoreRandomUser,
                Description = GenerateRandomString()
            }).Result;

            var randomTask = await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = oneMoreRandomUser,
                TargetUser = randomUser,
                Description = GenerateRandomString()
            });

            await taskRepository.SaveChangesAsync();

            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { CreatorUser = randomUser.Id }).Result.Data.Count(), 2);
            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { CreatorUser = anotherRandomUser.Id }).Result.Data.Count(), 0);
            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.OR }).Result.Data.Count(), 3);
            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.AND }).Result.Data.Count(), 0);

            var anotherRandomTask = await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = oneMoreRandomUser,
                TargetUser = oneMoreRandomUser,
                Description = GenerateRandomString()
            });

            await taskRepository.SaveChangesAsync();

            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { TargetUser = oneMoreRandomUser.Id, CreatorUser = oneMoreRandomUser.Id, UserFilter = FilterHelper.AND }).Result.Data.Count(), 1);
            Assert.AreEqual(taskRepository.GetAsync(null).Result.Data.Count(), 4);

            await taskRepository.FinishAsync(new UserTask() { TaskId = randomTask.Id, User = randomUser });
            await taskRepository.FinishAsync(new UserTask() { TaskId = anotherRandomTask.Id, User = oneMoreRandomUser });

            await taskRepository.SaveChangesAsync();

            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = true }).Result.Data.Count(), 2);
            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = false }).Result.Data.Count(), 2);
            Assert.AreEqual(taskRepository.GetAsync(new TaskFilter() { Completed = null }).Result.Data.Count(), 4);

            var date = DateTime.Today.AddDays(-2);

            var task1 = context.Task.Single(x => x.Id == someTask.Id);
            task1.AlterCompleteddAt(date);

            context.Task.UpdateRange(task1);
            context.SaveChanges();

            TaskFilter filter;
            PaginationResult<TaskResult> result;

            filter = new TaskFilter()
            {
                CompletedBetween = new Period(date.AddHours(-1), date.AddHours(1))
            };

            result = await taskRepository.GetAsync(filter);

            Assert.AreEqual(result.Data.Count(), 1);

            filter = new TaskFilter()
            {
                CompletedBetween = new Period(DateTime.Today, null)
            };

            result = await taskRepository.GetAsync(filter);

            Assert.AreEqual(result.Data.Count(), 2);
        }
    }
}