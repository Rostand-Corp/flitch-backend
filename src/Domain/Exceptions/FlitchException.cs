using System.Net;

namespace Domain.Exceptions;

public class FlitchException : Exception
{
    public FlitchException(string title, string errorType, string message, 
        HttpStatusCode httpStatusCode = HttpStatusCode.NotImplemented) : base(message)
    {
        Title = title;
        ErrorType = errorType;
        HttpStatusCode = httpStatusCode;
    }

    public string Title { get; }
    public string ErrorType { get; }
    public HttpStatusCode HttpStatusCode { get; }

    public override string Message => base.Message;
}