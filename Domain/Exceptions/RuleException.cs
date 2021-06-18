using System;

namespace Domains.Exceptions
{
	public class RuleException : Exception
	{
		#region Constructors

		public RuleException()
			: base()
		{ }

		public RuleException(string message)
			: base(message)
		{ }

		#endregion Constructors
	}
}