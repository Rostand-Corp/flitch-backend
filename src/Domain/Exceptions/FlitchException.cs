using System.Net;

namespace Domain.Exceptions;

public class FlitchException : Exception
{
    public FlitchException(string title, string errorType, string message) : base(message)
    {
        Title = title;
        ErrorType = errorType;
    }
    public string Title { get; }
    public string ErrorType { get; }
}