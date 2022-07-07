using System;

namespace Domains.Exceptions
{
	public class RuleException : Exception
	{
		public RuleException()
			: base()
		{ }

		public RuleException(string message)
			: base(message)
		{ }
	}
}