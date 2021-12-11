using Domains.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		
	[Route("Users/{targetUserId:Guid}/Tasks")]
	[ApiExplorerSettings(GroupName = "Tasks")]
	public class TasksController : ApiControllerBase
	{
		#region Properties

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
		/// Creates and set a task to a given user.
		/// Only admins can set tasks to users.
		/// </summary>
		/// <param name="targetUserId">User whom the task is going to be assigned.</param>
		/// <param name="description">Task's description.</param>
		/// <returns>Task details.</returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]		
		[ProducesDefaultResponseType]		
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<AssignTaskResult>> New([FromRoute] Guid targetUserId, [FromBody] string description)
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

				return result;
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

		#endregion Methods
	}
}
