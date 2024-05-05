using System;

namespace MeWhen.Domain.Exception
{
    public class BadRequestException(List<string> ErrorMessages) : System.Exception(string.Join("|", ErrorMessages))
    {
        public List<string> ListError { get; } = ErrorMessages;
    }
}
