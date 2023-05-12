using System.Net;

namespace Domain.Exceptions;

public class InvalidIdentifierException : FlitchException
{
    public InvalidIdentifierException() 
        : base(
            "Invalid identifier specified.",
            "Flitch.Id", 
            "The specified identifier is invalid"
            )
    {
    }
}