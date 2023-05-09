using System.Net;

namespace Domain.Exceptions;

public class NotFoundException : FlitchException
{
    public NotFoundException(string errorType, string message) 
        : base(
            "Specified resource was not found.",
            errorType, 
            message
        )
    {
    }
}