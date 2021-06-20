using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;

namespace ToDoList.UI.Controllers.Base
{
	[Authorize]
	[ApiController]
	[Produces("application/json")]
	public class ApiControllerBase : ControllerBase
	{
		#region Fields

		protected readonly IUserRepository repo;
		protected UserResult authenticatedUser;

		#endregion Fields

		#region Constructor

		public ApiControllerBase(IHttpContextAccessor httpContextAccessor, IUserRepository repo)
		{
			this.repo = repo;

			var canValidateAuthentication = httpContextAccessor.HttpContext.User.Claims.Any();
			if (!canValidateAuthentication) return;

			var claim = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid);
			var userId = new Guid(claim.Value);

			authenticatedUser = repo.Get(userId).Result;

			if (authenticatedUser == null) throw new InvalidCredentialException();
		}

		#endregion Constructor
	}
}