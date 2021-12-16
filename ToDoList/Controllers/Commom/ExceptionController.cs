using Domains.Exceptions;
using Microsoft.AspNetCore.Http;
using System;

namespace ToDoList.UI.Controllers.Commom
{
	internal class ExceptionController
	{
		public static int GetStatusCode(Exception exception)
		{
			if (exception is MissingArgumentsException)
				return StatusCodes.Status400BadRequest;
			if (exception is RuleException)
				return StatusCodes.Status422UnprocessableEntity;
			if (exception is NotFoundException)
				return StatusCodes.Status404NotFound;
			if (exception is PermissionException)
				return StatusCodes.Status403Forbidden;

			return StatusCodes.Status500InternalServerError;
		}
	}
}
