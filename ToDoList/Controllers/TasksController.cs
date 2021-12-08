using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using System.Threading.Tasks;
using ToDoList.UI.Controllers.Base;

namespace ToDoList.UI.Controllers
{
	#region Documentation

	/// <summary>
	/// Responsible class for tasks management.
	/// </summary>

	#endregion Documentation

	[Authorize]
	[Route("Tasks")]
	[ApiExplorerSettings(GroupName = "Users")]
	public class TasksController : ApiControllerBase
	{
		#region Constructor

		public TasksController(IHttpContextAccessor httpContextAccessor, IUserRepository repo)
			: base(httpContextAccessor, repo)
		{
		}

		#endregion Constructor

		#region Methods

		

		#endregion Methods
	}
}
