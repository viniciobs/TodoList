using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers.Base
{
	[Authorize]
	[ApiController]
	[Produces("application/json")]
	public class ApiControllerBase : ControllerBase
	{
		protected readonly IUserRepository _userRepo;
		protected User authenticatedUser;

		public ApiControllerBase(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo)
		{
			_userRepo = userRepo;

			authenticatedUser = httpContextAccessor.GetAuthenticatedUser(userRepo);
		}
	}
}