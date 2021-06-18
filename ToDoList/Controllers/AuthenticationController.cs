using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.DTOs.Users;
using Repository.Exceptions;
using Repository.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoList.UI.Configurations;

namespace ToDoList.UI.Controllers
{
	#region Documentation

	/// <summary>
	/// Responsible class for users authentications.
	/// </summary>

	#endregion Documentation

	[Produces("application/json")]
	[Route("authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		#region Fields

		private readonly IUserRepository repo;
		private readonly Authentication authentication;

		#endregion Fields

		#region Constructor

		public AuthenticationController(IUserRepository repo, IOptions<Authentication> authentication)
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
		[ProducesDefaultResponseType]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationData))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(AuthenticationData))]
		public async Task<ActionResult<AuthenticationResult>> Login(AuthenticationData data)
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
			catch (NotFoundException notFoundException)
			{
				return NotFound(notFoundException);
			}
			catch (Exception exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
			}

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

		#endregion Methods
	}
}