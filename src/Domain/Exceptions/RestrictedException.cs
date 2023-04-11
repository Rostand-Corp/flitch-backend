using System.Net;

namespace Domain.Exceptions;

public class RestrictedException : FlitchException
{
    public RestrictedException(string errorType, string message, HttpStatusCode httpStatusCode = HttpStatusCode.Forbidden) : base(errorType, message, httpStatusCode)
    {
    }
}