using System.Net;

namespace Domain.Exceptions;

public class ValidationException : FlitchException
{
    public ValidationException(string domain, IDictionary<string, string[]> errors)
        : base(
            "Validation problem has occured.",
            domain + ".Validation",
            "Validation problem has occurred."
            )
    {
        Errors = errors;
    }
    
    public IDictionary<string, string[]> Errors { get;}
}