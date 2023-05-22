using System.Runtime.Serialization;

namespace Example.Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        protected BaseException()
        { }

        protected BaseException(string message)
            : base(message)
        { }

        protected BaseException(string message, object[] args)
          : base(GetMessage(message, args))
        { }

        protected BaseException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected BaseException(string message, object[] args, Exception innerException)
           : base(GetMessage(message, args), innerException)
        { }

        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        private static string GetMessage(string message, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(message) || !(args?.Any() ?? false))
            {
                return message;
            }

            var formattedMessage = string.Format(message, args);

            return formattedMessage;
        }
    }
}
