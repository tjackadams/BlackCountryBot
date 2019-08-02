using System;

namespace Bot.Domain.Exceptions
{
    public class BotDomainException : Exception
    {
        public BotDomainException() { }
        public BotDomainException(string message)
        : base(message)
        {
        }
        public BotDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
