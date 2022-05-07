using System;

namespace Domains.Exceptions
{
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException()
            : base()
        { }

        public UnprocessableEntityException(string message)
            : base(message)
        { }
    }
}