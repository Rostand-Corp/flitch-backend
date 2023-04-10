using System.Net;

namespace Domain.Exceptions;

public class ValidationException : FlitchException
{
    public ValidationException(string domain, IDictionary<string, string[]> errors)
        : base(domain + ".Validation", "Validation error has occurred.", HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
    
    public IDictionary<string, string[]> Errors { get;}
}