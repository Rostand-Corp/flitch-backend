using System.Net;

namespace Domain.Exceptions;

public class AlreadyExistsException : FlitchException
{
    public AlreadyExistsException(string errorType, string message) 
        : base(
            "The resource already exists.",
            errorType, 
            message, 
            HttpStatusCode.Conflict
            )
    {
    }
}