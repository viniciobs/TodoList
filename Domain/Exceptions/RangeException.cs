using System;

namespace Domains.Exceptions
{
	public class RangeException : ArgumentOutOfRangeException
	{
		#region Constructors

		public RangeException()
			: base()
		{ }

		public RangeException(string paramName, int? minLength, int? maxLength)
			: base(paramName, GetRangeExceptionMessage(minLength, maxLength))
		{ }

		#endregion Constructors

		#region Methods

		private static string GetRangeExceptionMessage(int? minLength = null, int? maxLength = null)
		{
			if (minLength != null && maxLength != null)
				return $"The value must be between {minLength} and {maxLength}";
			else if (minLength != null)
				return $"The value must be higher than {minLength}";
			else
				return $"The value must be smaller than {maxLength}";
		}

		#endregion Methods
	}
}