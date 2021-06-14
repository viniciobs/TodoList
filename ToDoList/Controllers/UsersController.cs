using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.UI.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Repository.DTOs.Users;
using Repository.Interfaces;

namespace ToDoList.UI.Controllers
{
	#region Documentation

	/// <summary>
	/// Responsible class for users management.
	/// </summary>

	#endregion Documentation

	[Route("Users")]
	public class UsersController : ApiControllerBase
	{
		#region Constructor

		public UsersController(IHttpContextAccessor httpContextAccessor, IUserRepository repo)
			: base(httpContextAccessor, repo)
		{
		}

		#endregion Constructor

		#region Methods

		#region Documentation

		/// <summary>
		/// List users.
		/// Can be filtered by username and login.
		/// </summary>
		/// <returns>A list of users.</returns>

		#endregion Documentation

		[HttpGet]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<UserResult>>> Get(string name, string login)
		{
			try
			{
				return await repo.Get(name: name, login: login);
			}
			catch (Exception exception)
			{
				ModelState.AddModelError(string.Empty, exception.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
			}
		}

		#region Documentation

		/// <summary>
		/// Show details of a specific user.
		/// </summary>
		/// <param name="id">User id.</param>
		/// <returns>User details.</returns>

		#endregion Documentation

		[HttpGet]
		[Route("{id:Guid}")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UserResult))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<UserResult>> Get(Guid id)
		{
			UserResult user;

			try
			{
				user = await repo.Get(id);
			}
			catch (Exception exception)
			{
				ModelState.AddModelError(string.Empty, exception.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
			}

			if (user == null) return NotFound(user);

			return user;
		}

		#region Documentation

		/// <summary>
		/// Change a user password.
		/// </summary>
		/// <param name="id">The target user id.</param>
		/// <param name="data">Necessary data to change the user's password.</param>

		#endregion Documentation

		[HttpPatch]
		[Route("{id:Guid}/ChangePassword")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UserResult))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordData data)
		{
			if (data == null) return BadRequest(nameof(data));
			if (string.IsNullOrEmpty(data.OldPassword)) return BadRequest(nameof(data.OldPassword));
			if (string.IsNullOrEmpty(data.NewPassword)) return BadRequest(nameof(data.NewPassword));

			if (id != authenticatedUser.Id)
			{
				ModelState.AddModelError("User", "The given identifier mismatch the authenticated user");

				return Conflict(ModelState);
			}

			try
			{
				await repo.ChangePassword(id, data);
				await repo.SaveChangesAsync();
			}
			catch (ApplicationException applicationException)
			{
				ModelState.AddModelError("User", applicationException.Message);

				return NotFound(ModelState);
			}
			catch (Exception exception)
			{
				ModelState.AddModelError("User", exception.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
			}

			return NoContent();
		}

		#region Documentation

		/// <summary>
		/// Create a user.
		/// No authentication required.
		/// </summary>
		/// <param name="data">Necessary data to create a user.</param>
		/// <returns>User details.</returns>

		#endregion Documentation

		[HttpPost]
		[AllowAnonymous]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserResult))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<ActionResult<CreateUserResult>> New([FromBody] CreateUserData data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			CreateUserResult result;

			try
			{
				result = await repo.Create(data);

				await repo.SaveChangesAsync();
			}
			catch (ApplicationException applicationException)
			{
				ModelState.AddModelError(nameof(CreateUserData), applicationException.Message);

				return UnprocessableEntity(ModelState);
			}
			catch (ArgumentNullException argumentNullException)
			{
				ModelState.AddModelError(argumentNullException.ParamName, argumentNullException.Message);

				return BadRequest(ModelState);
			}
			catch (Exception exception)
			{
				ModelState.AddModelError(string.Empty, exception.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
			}

			return Created(nameof(Get), result);
		}

		#region Documentation

		/// <summary>
		/// Delete a user.
		/// Only users with admin roles can delete a user.
		/// </summary>
		/// <param name="id">User id.</param>

		#endregion Documentation

		[HttpDelete]
		[Route("{id:Guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				await repo.Delete(id);
				await repo.SaveChangesAsync();
			}
			catch (ApplicationException applicationException)
			{
				ModelState.AddModelError(nameof(User), applicationException.Message);

				return NotFound(ModelState);
			}
			catch (Exception exception)
			{
				ModelState.AddModelError(string.Empty, exception.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
			}
			//var strategy = context.Database.CreateExecutionStrategy();

			//await strategy.ExecuteAsync(async () =>
			//{
			//	await using var transaction = await context.Database.BeginTransactionAsync();

			//	try
			//	{
			//		var targetTasks = context.Entry(user).Collection(x => x.TargetTasks).Query().Select(x => x);
			//		context.Task.RemoveRange(targetTasks);

			//		var createdTasks = context.Entry(user).Collection(x => x.CreatedTasks).Query().Select(x => x);
			//		context.Task.RemoveRange(createdTasks);

			//		context.User.Remove(user);

			//		await context.SaveChangesAsync();
			//		await transaction.CommitAsync();
			//	}
			//	catch (Exception exception)
			//	{
			//		throw new ApplicationException("Could not delete user. \n" + exception.Message);
			//	}
			//});

			return NoContent();
		}

		#endregion Methods
	}
}