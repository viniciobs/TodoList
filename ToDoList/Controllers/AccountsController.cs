using Domains.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.DTOs.Accounts;
using Repository.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoList.UI.Configurations;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers
{
	#region Documentation

	/// <summary>
	/// Responsible class for account management
	/// </summary>

	#endregion Documentation

	[Produces("application/json")]
	[Route("accounts")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "Accounts")]
	public class AccountsController : ControllerBase
	{
		#region Fields

		private readonly IAccountRepository repo;
		private readonly Authentication authentication;

		#endregion Fields

		#region Constructor

		public AccountsController(IAccountRepository repo, IOptions<Authentication> authentication)
		{
			this.repo = repo;
			this.authentication = authentication.Value;
		}

		#endregion Constructor

		#region Methods

		#region Documentation

		/// <summary>
		/// Identify, authenticate and generate a token to a user.
		/// The token will be available for the next 30 minutes.
		/// </summary>
		/// <param name="data">Necessary data to authenticate.</param>
		/// <returns>If authenticated, return the user data.</returns>

		#endregion Documentation

		[HttpPost]
		[Route("Authenticate")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationData))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(AuthenticationData))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<AuthenticationResult>> Authenticate(AuthenticationData data)
		{
			if (data == null || string.IsNullOrEmpty(data.Login) || string.IsNullOrEmpty(data.Password))
				return BadRequest("Authentication data not received correctly");

			var expireMinutes = 30;

#if DEBUG
			expireMinutes = 60 * 24;
#endif
			AuthenticationResult authenticationResult;

			try
			{
				authenticationResult = await repo.Authenticate(data);
			}
			catch (MissingArgumentsException missingArgumentException)
			{
				return BadRequest(missingArgumentException);
			}
			catch (NotFoundException notFoundException)
			{
				return NotFound(notFoundException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.UTF8.GetBytes(authentication.Secret);

				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.Sid, authenticationResult.UserId.ToString()),
					new Claim(ClaimTypes.Role, authenticationResult.Role.ToString())
					}),
					Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
				};

				var token = tokenHandler.CreateToken(tokenDescriptor);
				authenticationResult.Token = tokenHandler.WriteToken(token);

				return Ok(authenticationResult);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}
		}

		#region Documentation

		/// <summary>
		/// Validate, create and authenticate a new account.
		/// </summary>
		/// <param name="data">Necessary data to create an account.</param>
		/// <returns>Account details.</returns>

		#endregion Documentation

		[HttpPost]
		[Route("New")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthenticationResult))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<ActionResult<AuthenticationResult>> New([FromBody] CreateAccountData data)
		{
			try
			{
				await repo.Create(data);
				await repo.SaveChangesAsync();
			}
			catch (RuleException ruleException)
			{
				return UnprocessableEntity(ruleException);
			}
			catch (MissingArgumentsException missingArgumentsException)
			{
				return BadRequest(missingArgumentsException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}

			var authenticationData = new AuthenticationData()
			{
				Login = data.Login,
				Password = data.Password
			};

			return await Authenticate(authenticationData);
		}

		#region Documentation

		/// <summary>
		/// Change a user password.
		/// </summary>
		/// <param name="id">The target user id.</param>
		/// <param name="data">Necessary data to change the user's password.</param>

		#endregion Documentation

		[Authorize]
		[HttpPatch]
		[Route("{id:Guid}/ChangePassword")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ChangePassword(
			[FromRoute] Guid id,
			[FromBody] ChangePasswordData data,
			[FromServices] IHttpContextAccessor httpContextAccessor,
			[FromServices] IUserRepository userRepository
			)
		{
			if (data == null) return BadRequest(nameof(data));
			if (string.IsNullOrEmpty(data.OldPassword)) return BadRequest(nameof(data.OldPassword));
			if (string.IsNullOrEmpty(data.NewPassword)) return BadRequest(nameof(data.NewPassword));

			var authenticatedUser = httpContextAccessor.GetAuthenticatedUser(userRepository);

			if (id != authenticatedUser.Id)
				return Conflict(new Exception("The given identifier mismatch the authenticated user"));

			try
			{
				await repo.ChangePassword(id, data);
				await repo.SaveChangesAsync();
			}
			catch (MissingArgumentsException missingArgumentsException)
			{
				return BadRequest(missingArgumentsException);
			}
			catch (NotFoundException notFoundException)
			{
				return NotFound(notFoundException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}

			return NoContent();
		}

		#region Documentation

		/// <summary>
		/// Delete a user.
		/// Only users with admin roles can delete a user.
		/// </summary>
		/// <param name="id">User id.</param>

		#endregion Documentation

		[Authorize]
		[HttpDelete]
		[Route("{id:Guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Delete(
			[FromRoute] Guid id,
			[FromServices] IHttpContextAccessor httpContextAccessor,
			[FromServices] IUserRepository userRepository
			)
		{
			try
			{
				await repo.Delete(id);
				await repo.SaveChangesAsync();
			}
			catch (NotFoundException notFoundException)
			{
				return NotFound(notFoundException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception);
			}

			#region TODO

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

			#endregion TODO

			return NoContent();
		}

		#endregion Methods
	}
}