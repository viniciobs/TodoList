using Microsoft.AspNetCore.Http;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;

namespace ToDoList.UI.Controllers
{
	internal static class UtilController
	{
		public static UserResult GetAuthenticatedUser(this IHttpContextAccessor httpContextAccessor, IUserRepository repo)
		{
			var canValidateAuthentication = httpContextAccessor.HttpContext.User.Claims.Any();
			if (!canValidateAuthentication) return null;

			var claim = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid);
			var userId = new Guid(claim.Value);

			return repo.Get(userId).Result;
		}
	}
}