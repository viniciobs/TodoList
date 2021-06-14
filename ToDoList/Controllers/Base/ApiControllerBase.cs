using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System.Security.Claims;
using System.Security.Authentication;

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