using System;

namespace Domains.Exceptions
{
	public class PermissionException : Exception
	{	
		public PermissionException()
			: base()
		{ }

		public PermissionException(string message)
			: base(message)
		{ }
	}
}