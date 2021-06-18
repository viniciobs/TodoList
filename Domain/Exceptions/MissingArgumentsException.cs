using System;

namespace Domains.Exceptions
{
	public class MissingArgumentsException : ArgumentNullException
	{
		#region Constructors

		public MissingArgumentsException()
			: base()
		{ }

		public MissingArgumentsException(string paramName)
			: base(paramName)
		{ }

		public MissingArgumentsException(string paramName, string message = null)
			: base(paramName, message)
		{ }

		#endregion Constructors
	}
}