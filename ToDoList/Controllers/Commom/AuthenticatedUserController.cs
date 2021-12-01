using Microsoft.AspNetCore.Http;
using Repository.DTOs.Users;
using Repository.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;

namespace ToDoList.UI.Controllers.Commom
{
	internal static class AuthenticatedUserController
	{
		public static UserResult GetAuthenticatedUser(this IHttpContextAccessor httpContextAccessor, IUserRepository repo)
		{
			var canValidateAuthentication = httpContextAccessor.HttpContext.User.Claims.Any();
			if (!canValidateAuthentication) return null;

			var claim = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid);
			var userId = new Guid(claim.Value);

			try
			{
				return repo.Get(userId)?.Result;
			}
			catch
			{
				return null;
			}
		}
	}
}