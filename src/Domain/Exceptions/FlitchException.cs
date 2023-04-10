using System.Net;

namespace Domain.Exceptions;

public class FlitchException : Exception
{
    public FlitchException(string errorType, string message, 
        HttpStatusCode httpStatusCode = HttpStatusCode.NotImplemented) : base(message)
    {
        ErrorType = errorType;
        HttpStatusCode = httpStatusCode;
    }

    public string ErrorType { get; }
    public HttpStatusCode HttpStatusCode { get; }

    public override string Message => base.Message;
}