using System;

namespace Repository.Exceptions
{
	public class NotFoundException : Exception
	{
		#region Properties

		public Type Type { get; private set; }

		#endregion Properties

		#region Constructors

		public NotFoundException()
			: base()
		{ }

		public NotFoundException(string exceptionMessage)
			: base(exceptionMessage)
		{ }

		public NotFoundException(Type type)
			: base($"{type.Name} not found")
		{
			Type = type;
		}

		public NotFoundException(Type type, string exceptionMessage)
			: base(exceptionMessage)
		{
			Type = type;
		}

		#endregion Constructors
	}
}