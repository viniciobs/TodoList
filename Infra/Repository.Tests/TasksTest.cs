using Domains;
using Domains.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Tasks;
using Repository.Pagination;
using Repository.Tests.Base;
using Repository.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Repository.Tests
{
    public class TasksTest : RepositoryTestBase
    {
        [Fact]
        public async Task AssignToNullTargetUser_ThrowMissingArgumentsException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                Description = "Test",
                TargetUser = null
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
        public async Task AssignToNullCreatorUser_ThrowMissingArgumentsException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = null,
                Description = "Test",
                TargetUser = adminUser
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task AssignWithNUllDescription_ThrowMissingArgumentsException(string description)
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                Description = description,
                TargetUser = normalUser
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
        public async Task AssignToInactiveTargetUser_ThrowRuleException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = adminUser,
                TargetUser = GenerateDeactivatedUser(),
                Description = "Test"
            };

            await Assert.ThrowsAsync<RuleException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
        public async Task AssignToInactiveCreatorUser_ThrowRuleException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = GenerateDeactivatedUser(),
                TargetUser = adminUser,
                Description = "Test"
            };

            await Assert.ThrowsAsync<RuleException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
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

            Assert.Null(task.CompletedAt);
        }

        [Fact]
        public async Task TryAssignToNullTargetUser_ThrowNotFoundException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = normalUser,
                TargetUser = null,
                Description = "Test"
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
        public async Task TryAssignToNullCreatorUser_ThrowNotFoundException()
        {
            var assignData = new AssignTaskData()
            {
                CreatorUser = null,
                TargetUser = normalUser,
                Description = "Test"
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.AssignAsync(assignData));
        }

        [Fact]
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

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.FinishAsync(data));
        }

        [Fact]
        public async Task TryFinishTaskWithInvalidTaskId_ThrowMissingArgumentException()
        {
            var data = new UserTask()
            {
                TaskId = default,
                User = normalUser
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.FinishAsync(data));
        }

        [Fact]
        public async Task TryFinishTaskWithInvalidTaskId_ThrowNotFoundException()
        {
            var data = new UserTask()
            {
                User = adminUser,
                TaskId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await taskRepository.FinishAsync(data));
        }

        [Fact]
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

            Assert.NotNull(finishedTask.CompletedAt);
        }

        [Fact]
        public async Task TryReopenWithNullUser_ThrowMissingArgumentException()
        {
            var taskToFinish = normalUser.SelfAssignTask("Test");
            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                User = null,
                TaskId = taskToFinish.Id
            };

            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await taskRepository.ReopenAsync(data));
        }

        [Fact]
        public async Task TryReopenWithInvalidUser_ThrowNotFoundException()
        {
            var taskToFinish = normalUser.SelfAssignTask("Test");
            await taskRepository.SaveChangesAsync();

            var data = new UserTask()
            {
                User = GenerateRandomUser(),
                TaskId = taskToFinish.Id
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await taskRepository.ReopenAsync(data));
        }

        [Fact]
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

            Assert.NotNull(task.CompletedAt);

            await taskRepository.ReopenAsync(data);
            await taskRepository.SaveChangesAsync();

            task = context.Task.Single(x => x.Id == task.Id);

            Assert.Null(task.CompletedAt);
        }

        [Fact]
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

            var someTask = await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = randomUser,
                TargetUser = oneMoreRandomUser,
                Description = GenerateRandomString()
            });

            var randomTask = await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = oneMoreRandomUser,
                TargetUser = randomUser,
                Description = GenerateRandomString()
            });

            await taskRepository.SaveChangesAsync();

            Assert.Equal(2, (await taskRepository.GetAsync(new TaskFilter() { CreatorUser = randomUser.Id })).Data.Count());
            Assert.Empty((await taskRepository.GetAsync(new TaskFilter() { CreatorUser = anotherRandomUser.Id })).Data);
            Assert.Equal(3, (await taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.OR })).Data.Count());
            Assert.Empty((await taskRepository.GetAsync(new TaskFilter() { TargetUser = randomUser.Id, CreatorUser = randomUser.Id, UserFilter = FilterHelper.AND })).Data);

            var anotherRandomTask = await taskRepository.AssignAsync(new AssignTaskData()
            {
                CreatorUser = oneMoreRandomUser,
                TargetUser = oneMoreRandomUser,
                Description = GenerateRandomString()
            });

            await taskRepository.SaveChangesAsync();

            Assert.Single((await taskRepository.GetAsync(new TaskFilter() { TargetUser = oneMoreRandomUser.Id, CreatorUser = oneMoreRandomUser.Id, UserFilter = FilterHelper.AND })).Data);
            Assert.Equal(4, (await taskRepository.GetAsync(null)).Data.Count());

            await taskRepository.FinishAsync(new UserTask() { TaskId = randomTask.Id, User = randomUser });
            await taskRepository.FinishAsync(new UserTask() { TaskId = anotherRandomTask.Id, User = oneMoreRandomUser });

            await taskRepository.SaveChangesAsync();

            Assert.Equal(2, (await taskRepository.GetAsync(new TaskFilter() { Completed = true })).Data.Count());
            Assert.Equal(2, (await taskRepository.GetAsync(new TaskFilter() { Completed = false })).Data.Count());
            Assert.Equal(4, (await taskRepository.GetAsync(new TaskFilter() { Completed = null })).Data.Count());

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

            Assert.Single(result.Data);

            filter = new TaskFilter()
            {
                CompletedBetween = new Period(DateTime.Today, null)
            };

            result = await taskRepository.GetAsync(filter);

            Assert.Equal(2, result.Data.Count());
        }
    }
}