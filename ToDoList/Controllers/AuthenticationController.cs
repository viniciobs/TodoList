using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Repository.Interfaces;
using Repository.DTOs.Users;
using ToDoList.UI.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

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
			if (data == null || data.Login == null || data.Password == null)
			{
				ModelState.AddModelError(string.Empty, "Authentication data not received correctly");

				return BadRequest(ModelState);
			}

			var expireMinutes = 30;

#if DEBUG
			expireMinutes = 60 * 24;
#endif
			var authenticationResult = await repo.Authenticate(data);
			var isValid = authenticationResult != null;

			if (!isValid)
			{
				ModelState.AddModelError(string.Empty, "Invalid credentials");

				return NotFound(ModelState);
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