using System;

namespace MeWhen.Domain.Exception
{
    public class BadRequestException(string errorMessage) : System.Exception(errorMessage)
    {
        public string ListError { get; } = errorMessage;
    }
}
