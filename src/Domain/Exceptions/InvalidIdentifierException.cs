using System.Net;

namespace Domain.Exceptions;

public class InvalidIdentifierException : FlitchException
{
    public InvalidIdentifierException() : base("Flitch.Id", "The specified identifier is invalid", HttpStatusCode.BadRequest)
    {
    }
}