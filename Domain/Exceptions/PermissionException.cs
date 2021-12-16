using System;

namespace Domains.Exceptions
{
	public class PermissionException : Exception
	{
		#region Constructors

		public PermissionException()
			: base()
		{ }

		public PermissionException(string message)
			: base(message)
		{ }

		#endregion Constructors
	}
}