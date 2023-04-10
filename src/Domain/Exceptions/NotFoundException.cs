using System.Net;

namespace Domain.Exceptions;

public class NotFoundException : FlitchException
{
    public NotFoundException(string errorType, string message) : base(errorType, message, HttpStatusCode.NotFound)
    {
    }
}