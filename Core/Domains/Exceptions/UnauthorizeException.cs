using System;

namespace Domains.Exceptions
{
	public class UnauthorizeException : Exception
	{
		public UnauthorizeException()
		{}

		public UnauthorizeException(string message)
			: base(message)
		{ }
	}
}
