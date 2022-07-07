using Domains;
using Domains.Exceptions;
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

				return repo.FindAsync(userId)?.Result;
			}
			catch
			{
				return null;
			}
		}

		public static User EnsureAuthentication(this IHttpContextAccessor httpContextAccessor, IUserRepository repo)
		{
			User user = GetAuthenticatedUser(httpContextAccessor, repo);
			if (user == null) throw new UnauthorizeException("You must be logged in to procceed");

			return user;
		}
	}
}