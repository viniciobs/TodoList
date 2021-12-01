using Domains.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.Users;
using Repository.Interfaces;
using ToDoList.UI.Controllers.Commom;

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

			authenticatedUser = httpContextAccessor.GetAuthenticatedUser(repo);
		}

		#endregion Constructor
	}
}