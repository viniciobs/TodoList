using Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.History;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
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
		private readonly IHistoryRepository _historyRepository;

		public TaskCommentsController(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo, ITaskCommentRepository repo, IHistoryRepository historyRepository)
			: base(httpContextAccessor, userRepo)
		{
			_repo = repo;
			_historyRepository = historyRepository;
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
			try
			{
				TaskCommentData data = new TaskCommentData()
				{
					Comment = comment,
					TaskId = id,
					User = authenticatedUser
				};

				TaskCommentResult result = await _repo.AddCommentAsync(data);
				await _repo.SaveChangesAsync();

				var historyData = new AddHistoryData()
				{
					UserId = authenticatedUser.Id,
					Action = HistoryAction.AddedCommentToTask,
					Content = new { TaskId = id, Comment = comment }
				};

				_historyRepository.AddHistoryAsync(historyData);

				return StatusCode(StatusCodes.Status201Created, result);
			}
			catch (Exception exception)
			{
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
			PaginationResult<TaskCommentResult> result;

			try
			{
				TaskCommentFilter filter = new TaskCommentFilter()
				{
					TaskId = id,
					CreatedBetween = new Period(start, end),
					Page = page,
					ItemsPerPage = itemsPerPage
				};

				result = await _repo.GetAsync(filter);

				var historyData = new AddHistoryData()
				{
					UserId = authenticatedUser.Id,
					Action = HistoryAction.ListedTaskComments,
					Content = new { Filter = filter }
				};

				_historyRepository.AddHistoryAsync(historyData);

				return Ok(result);
			}
			catch (Exception exception)
			{
				int code = ExceptionController.GetStatusCode(exception);
				return StatusCode(code, exception);
			}
		}
	}
}
