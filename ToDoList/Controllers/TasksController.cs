using Domains.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;
using ToDoList.UI.Controllers.Base;

namespace ToDoList.UI.Controllers
{
	#region Documentation

	/// <summary>
	/// Responsible class for tasks management.
	/// </summary>

	#endregion Documentation
			
	[ApiExplorerSettings(GroupName = "Tasks")]
	public class TasksController : ApiControllerBase
	{
		#region Properties

		private const string BASE_ROUTE = "Users/{targetUserId:Guid}/Tasks";
		private readonly ITaskRepository _repo;

		#endregion Properties

		#region Constructor

		public TasksController(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo, ITaskRepository repo)
			: base(httpContextAccessor, userRepo)
		{
			_repo = repo;
		}

		#endregion Constructor

		#region Methods

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
		public async Task<ActionResult<TaskResult>> New([FromRoute] Guid targetUserId, [FromBody] string description)
		{		
			try
			{
				var targetUser = await _userRepo.Find(targetUserId);				

				var assignData = new AssignTaskData()
				{
					CreatorUser = authenticatedUser,
					TargetUser = targetUser,
					Description = description
				};

				var result = await _repo.Assign(assignData);
				await _repo.SaveChangesAsync();

				return StatusCode(StatusCodes.Status201Created, result);
			}
			catch(PermissionException permissionException)
			{
				return Unauthorized(permissionException);
			}
			catch(MissingArgumentsException missingArgumentsException)
			{
				return BadRequest(missingArgumentsException);
			}	
			catch(NotFoundException notfoundException)
			{
				return NotFound(notfoundException);
			}
			catch(Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}
		}

		/// <summary>
		/// Retrives details about a task.
		/// </summary>
		/// <param name="targetUserId">User tied to the task</param>
		/// <param name="id">Task identifier</param>
		/// <returns></returns>		
		[Route(BASE_ROUTE + "{id:Guid}")]
		[HttpGet]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<TaskResult>> Find([FromRoute] Guid targetUserId, [FromRoute] Guid id)
		{
			if (authenticatedUser.Id != targetUserId && authenticatedUser.Role != Domains.UserRole.Admin) return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to see this task details");

			try
			{
				var result = await _repo.Find(targetUserId, id);

				return Ok(result);
			}
			catch (NotFoundException notfoundException)
			{
				return NotFound(notfoundException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}
		}

		[Route("Tasks")]
		[HttpGet]
		[Authorize(Roles = "Admin")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<PaginationResult<TaskResult>>> Get(bool? completed, Guid? creatorUser, Guid? targetUser, DateTime? start, DateTime? end, int page, int itemsPerPage)
		{
			try
			{
				var filter = new TaskFilter()
				{
					Completed = completed,
					CreatorUser = creatorUser,
					TargetUser = targetUser,
					CompletedBetween = new Period(start, end),
					Page = page,
					ItemsPerPage = itemsPerPage
				};

				var result = await _repo.Get(filter);

				return Ok(result);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}
		}

		#endregion Methods
	}
}
