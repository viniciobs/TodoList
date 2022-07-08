using Domains;
using Domains.Logger;
using Domains.Services.MessageBroker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
using Repository.Pagination;
using System;
using System.Threading.Tasks;
using ToDoList.UI.Controllers.Base;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers
{
    /// <summary>
    /// Responsible class for tasks comments management.
    /// </summary>
    [ApiExplorerSettings(GroupName = "task-comments")]
    [Route("Tasks/{id:Guid}/Comments")]
    public class TaskCommentsController : ApiControllerBase
    {
        private readonly ITaskCommentRepository _repo;
        private readonly IHistoryMessageBrokerProducer _historyService;
        private readonly ILogger _logger;

        public TaskCommentsController(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo, ITaskCommentRepository repo, IHistoryMessageBrokerProducer historyRepository, ILogger<TaskCommentsController> logger)
            : base(httpContextAccessor, userRepo)
        {
            _repo = repo;
            _historyService = historyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Adds a comment to a given task.
        /// </summary>
        /// <param name="id">The target task</param>
        /// <param name="comment">The comment to be made</param>
        /// <returns>Comment details</returns>
        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskCommentResult>> AddComment([FromRoute] Guid id, [FromBody] string comment)
        {
            LogRequest(_logger);

            var data = new TaskCommentData()
            {
                Comment = comment,
                TaskId = id,
                User = authenticatedUser
            };

            try
            {
                TaskCommentResult result = await _repo.AddCommentAsync(data);
                await _repo.SaveChangesAsync();

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, "Successfully added task comment", result).Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.AddedCommentToTask, new { TaskId = id, Comment = comment });

                await _historyService.PostHistoryAsync(historyData);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, "Adding comment failed", data).Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// List task comments made by current user.
        /// </summary>
        /// <param name="start">To filter completed date period. Optional.</param>
        /// <param name="end">To filter completed date period. Optional.</param>
        /// <param name="page">Filter page. Optional.</param>
        /// <param name="itemsPerPage">Items quantity per result. Optional.</param>
        /// <returns>Task comments</returns>
        [HttpGet]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationResult<TaskCommentResult>>> GetComments([FromRoute] Guid id, DateTime? start, DateTime? end, int page, int itemsPerPage)
        {
            LogRequest(_logger);

            PaginationResult<TaskCommentResult> result;

            var filter = new TaskCommentFilter()
            {
                TaskId = id,
                CreatedBetween = new Period(start, end),
                Page = page,
                ItemsPerPage = itemsPerPage
            };

            try
            {
                result = await _repo.GetAsync(filter);

                _logger.LogInformation(new LogContent(authenticatedUser.Id, ipAddress, "Listing tasks comments.", filter).Serialized());

                var historyData = new HistoryData(authenticatedUser.Id, HistoryAction.ListedTaskComments, new { Filter = filter });

                await _historyService.PostHistoryAsync(historyData);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, new LogContent(authenticatedUser.Id, ipAddress, "Error getting tasks comments", filter).Serialized());

                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }
    }
}