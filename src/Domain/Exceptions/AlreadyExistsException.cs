using System.Net;

namespace Domain.Exceptions;

public class AlreadyExistsException : FlitchException
{
    public AlreadyExistsException(string errorType, string message) : base(errorType, message, HttpStatusCode.Conflict)
    {
    }
}