using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.History;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;
using ToDoList.UI.Controllers.Base;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers
{
	/// <summary>
	/// Responsible class for users management.
	/// </summary>
	[Route("Users")]
	[ApiExplorerSettings(GroupName = "users")]
	public class UsersController : ApiControllerBase
	{
		private readonly IHistoryRepository _historyRepository;

		public UsersController(IHttpContextAccessor httpContextAccessor, IUserRepository repo, IHistoryRepository historyRepository)
			: base(httpContextAccessor, repo)
		{
			_historyRepository = historyRepository;
		}		

		/// <summary>
		/// List users.
		/// Can be filtered by username and login. Users with admin role can filter also by user's status. Normal users use the active filter always with true.
		/// </summary>
		/// <param name="name">Filter name. Optional.</param>
		/// <param name="login">Filter login. Optional.</param>
		/// <param name="active">Filter active status. Optional and only admin users can use this param.</param>
		/// <param name="page">Filter page.</param>
		/// <param name="itemsPerPage">Items quantity per result.</param>
		/// <returns>A list of users.</returns>		
		[HttpGet]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<PaginationResult<UserResult>>> Get(string name, string login, bool? active, int page, int itemsPerPage)
		{
			try
			{
				if (authenticatedUser.Role != UserRole.Admin) active = true;

				var filter = new UserFilter()
				{
					Name = name,
					Login = login,
					IsActive = active,
					Page = page,
					ItemsPerPage = itemsPerPage
				};
				
				var users = await _userRepo.GetAsync(filter);

				var historyData = new AddHistoryData()
				{
					UserId = authenticatedUser.Id,
					Action = HistoryAction.ListedUsers,
					Content = new { Filter = filter }
				};

				_historyRepository.AddHistoryAsync(historyData);

				return Ok(users);
			}
			catch (Exception exception)
			{
				int code = ExceptionController.GetStatusCode(exception);
				return StatusCode(code, exception);
			}
		}

		/// <summary>
		/// Show details of a specific user.
		/// Only admins can see details of a deactivated user.
		/// </summary>
		/// <param name="id">User id.</param>
		/// <returns>User details.</returns>
		[HttpGet]
		[Route("{id:Guid}")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<UserResult>> Get(Guid id)
		{		
			try
			{
				bool? filterOnlyActive = null;

				if (authenticatedUser.Id == id)
					filterOnlyActive = false;
				else if (authenticatedUser.Role != UserRole.Admin)
					filterOnlyActive = true;

				var user = await _userRepo.FindAsync(id, filterOnlyActive);

				UserResult userResult = UserResult.Convert(user);

				var historyData = new AddHistoryData()
				{
					UserId = authenticatedUser.Id,
					Action = HistoryAction.ListedUsers,
					Content = new { Id = id }
				};

				_historyRepository.AddHistoryAsync(historyData);

				return Ok(userResult);
			}
			catch (Exception exception)
			{
				int code = ExceptionController.GetStatusCode(exception);
				return StatusCode(code, exception);
			}			
		}

		/// <summary>
		/// Alter a user role.
		/// Only admin can alter users role.
		/// A user can't change its own role.
		/// </summary>
		/// <param name="targetUserid">The identifier of the target user.</param>
		/// <param name="targetUserNewRole">The new role of the target user.</param>
		[HttpPatch]
		[Route("{targetUserid:Guid}/AlterRole")]
		[Authorize(Roles = "Admin")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> AlterUserRole(Guid targetUserid, [FromBody] UserRole targetUserNewRole)
		{
			try
			{
				var data = new AlterUserRoleData()
				{
					AuthenticatedUser = authenticatedUser.Id,
					TargetUser = targetUserid,
					NewRole = targetUserNewRole
				};

				await _userRepo.AlterUserRoleAsync(data);
				await _userRepo.SaveChangesAsync();

				var historyData = new AddHistoryData()
				{
					UserId = authenticatedUser.Id,
					Action = HistoryAction.AlteredUserRole,
					Content = new { TargetUserId = targetUserid, NewRole = targetUserNewRole }
				};

				_historyRepository.AddHistoryAsync(historyData);

				return NoContent();
			}
			catch (Exception exception)
			{
				int code = ExceptionController.GetStatusCode(exception);
				return StatusCode(code, exception);
			}			
		}
	}
}