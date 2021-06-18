using System.Security;

namespace Domains.Exceptions
{
	public class PermissionException : SecurityException
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