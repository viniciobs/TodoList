using System;

namespace Domains.Exceptions
{
	public class NotFoundException : Exception
	{		
		public Type Type { get; private set; }

		public NotFoundException()
			: base()
		{ }

		public NotFoundException(string message)
			: base(message)
		{ }

		public NotFoundException(Type type)
			: base($"{type.Name} not found")
		{
			Type = type;
		}

		public NotFoundException(Type type, string message)
			: base(message)
		{
			Type = type;
		}
	}
}