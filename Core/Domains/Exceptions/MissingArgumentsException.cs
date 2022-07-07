using System;

namespace Domains.Exceptions
{
	public class MissingArgumentsException : ArgumentNullException
	{
		public MissingArgumentsException()
			: base()
		{ }

		public MissingArgumentsException(string paramName)
			: base(paramName)
		{ }

		public MissingArgumentsException(string paramName, string message = null)
			: base(paramName, message)
		{ }
	}
}