using Domains;
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
		public static User GetAuthenticatedUser(this IHttpContextAccessor httpContextAccessor, IUserRepository repo)
		{
			var canValidateAuthentication = httpContextAccessor.HttpContext.User.Claims.Any();
			if (!canValidateAuthentication) return null;
			
			try
			{
				var claim = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid);
				var userId = new Guid(claim.Value);

				return repo.Find(userId)?.Result;
			}
			catch
			{
				return null;
			}
		}
	}
}