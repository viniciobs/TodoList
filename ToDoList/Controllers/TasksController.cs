using Domains;
using Domains.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;
using ToDoList.API.Services.MessageBroker.Sender;
using ToDoList.API.Services.MessageBroker.Sender.Models;
using ToDoList.UI.Controllers.Base;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers
{
    /// <summary>
    /// Responsible class for tasks management.
    /// </summary>
    [ApiExplorerSettings(GroupName = "tasks")]
    public class TasksController : ApiControllerBase
    {
        private const string BASE_ROUTE = "Users/{targetUserId:Guid}/Tasks";
        private readonly ITaskRepository _repo;
        private readonly IHistoryMessageBroker _historyService;
        private readonly ILogger _logger;

        public TasksController(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo, ITaskRepository repo, IHistoryMessageBroker historyService, ILogger<TasksController> logger)
            : base(httpContextAccessor, userRepo)
        {
            _repo = repo;
            _logger = logger;
            _historyService = historyService;
        }

        /// <summary>
        /// Creates and sets a task to a given user.
        /// Only admins can set tasks to users.
        /// </summary>
        /// <param name="targetUserId">User whom the task is going to be assigned.</param>
        /// <param name="description">Task's description.</param>
        /// <returns>Task details.</returns>
        [Route(BASE_ROUTE)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskResult>> AssignTo([FromRoute] Guid targetUserId, [FromBody] string description)
        {
            LogRequest(_logger);

            AssignTaskData assignData = null;

            try
            {
                var targetUser = await _userRepo.FindAsync(targetUserId);

                assignData = new AssignTaskData()
                {
                    CreatorUser = authenticatedUser,
                    TargetUser = targetUser,
                    Description = description
                };

                var result = await _repo.AssignAsync(assignData);
                await _repo.SaveChangesAsync();

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, $"Successfully assigned task to '{targetUserId}'.", assignData).Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.AssignedTask, new { TargetUserId = targetUserId, Description = description });

                await _historyService.PostHistoryAsync(historyData);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, $"Error assigning task to '{targetUserId}'.", assignData).Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Retrives details about a task.
        /// </summary>
        /// <param name="targetUserId">User tied to the task</param>
        /// <param name="id">Task identifier</param>
        /// <returns></returns>
        [Route(BASE_ROUTE + "/{id:Guid}")]
        [HttpGet]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskResult>> Find([FromRoute] Guid targetUserId, [FromRoute] Guid id)
        {
            LogRequest(_logger);

            if (authenticatedUser.Id != targetUserId && authenticatedUser.Role != UserRole.Admin) return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to see this task details");

            try
            {
                var result = await _repo.FindAsync(targetUserId, id);

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, "Successfully listed task details.").Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.ListedTasks, new { TargetUserId = targetUserId, TaskId = id });

                await _historyService.PostHistoryAsync(historyData);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, $"Error listing task '{id}' details for user '{targetUserId}'.").Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// List tasks.
        /// </summary>
        /// <param name="completed">If true, filter only completed tasks, otherwise, only pending tasks. Optional.</param>
        /// <param name="creatorUser">To filter creator user. Optional.</param>
        /// <param name="targetUser">To filter target user. Optional.</param>
        /// <param name="start">To filter completed date period. Optional.</param>
        /// <param name="end">To filter completed date period. Optional.</param>
        /// <param name="page">Filter page. Optional.</param>
        /// <param name="itemsPerPage">Items quantity per result. Optional.</param>
        /// <returns></returns>
        [Route("Admin/Tasks")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationResult<TaskResult>>> Get(bool? completed, Guid? creatorUser, Guid? targetUser, DateTime? start, DateTime? end, int page, int itemsPerPage)
        {
            LogRequest(_logger);

            TaskFilter filter = null;

            try
            {
                filter = new TaskFilter()
                {
                    Completed = completed,
                    CreatorUser = creatorUser,
                    TargetUser = targetUser,
                    UserFilter = FilterHelper.AND,
                    CompletedBetween = new Period(start, end),
                    Page = page,
                    ItemsPerPage = itemsPerPage
                };

                var result = await _repo.GetAsync(filter);

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, "Successfully listed tasks.", filter).Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.ListedTasks, new { Filter = filter });

                await _historyService.PostHistoryAsync(historyData);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, "Error listing tasks.", filter).Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// List user taks.
        /// </summary>
        /// <param name="completed">If true, filter only completed tasks, otherwise, only pending tasks. Optional.</param>
        /// <param name="start">To filter completed date period. Optional.</param>
        /// <param name="end">To filter completed date period. Optional.</param>
        /// <param name="page">Filter page. Optional.</param>
        /// <param name="itemsPerPage">Items quantity per result. Optional.</param>
        /// <returns>User tasks</returns>
        [Route("Tasks")]
        [HttpGet]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationResult<TaskResult>>> Get(bool? completed, DateTime? start, DateTime? end, int page, int itemsPerPage)
        {
            LogRequest(_logger);

            TaskFilter filter = null;

            try
            {
                filter = new TaskFilter()
                {
                    Completed = completed,
                    CreatorUser = authenticatedUser.Id,
                    TargetUser = authenticatedUser.Id,
                    UserFilter = FilterHelper.OR,
                    CompletedBetween = new Period(start, end),
                    Page = page,
                    ItemsPerPage = itemsPerPage
                };

                var result = await _repo.GetAsync(filter);

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, "Successfully listed tasks.", filter).Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.ListedTasks, new { Filter = filter });

                await _historyService.PostHistoryAsync(historyData);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, $"Error listing tasks.", filter).Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Finish a task.
        /// Only creator or target user can finish a task.
        /// </summary>
        /// <param name="id">Task to be finished.</param>
        [Route("Tasks/{id:Guid}/Finish")]
        [HttpPatch]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Finish(Guid id)
        {
            LogRequest(_logger);

            try
            {
                var data = new UserTask()
                {
                    TaskId = id,
                    User = authenticatedUser
                };

                await _repo.FinishAsync(data);
                await _repo.SaveChangesAsync();

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, $"Successfully finished task '{id}'.").Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.FinishedTask, new { TaskId = id });

                await _historyService.PostHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, $"Error finishing task '{id}'.").Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Reopen a task.
        /// Only creator or target user can reopen a task.
        /// </summary>
        /// <param name="id">Task to be reopened.</param>
        [Route("Tasks/{id:Guid}/Reopen")]
        [HttpPatch]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Reopen(Guid id)
        {
            LogRequest(_logger);

            try
            {
                var data = new UserTask()
                {
                    TaskId = id,
                    User = authenticatedUser
                };

                await _repo.ReopenAsync(data);
                await _repo.SaveChangesAsync();

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, $"Successfully reopened task '{id}'.").Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.ReopenedTask, new { TaskId = id });

                await _historyService.PostHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, $"Error reopening task '{id}'.").Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }
    }
}