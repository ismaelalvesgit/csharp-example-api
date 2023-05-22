using System.Runtime.Serialization;

namespace Example.Domain.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException()
        { }

        public NotFoundException(string message)
            : base(message)
        { }

        public NotFoundException(string message, object[] args)
            : base(message, args)
        { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public NotFoundException(string message, object[] args, Exception innerException)
            : base(message, args, innerException)
        { }

        public NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
