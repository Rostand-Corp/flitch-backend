using System.Net;
using Domain.Exceptions;

namespace Web.Services;

public static class HttpStatusCodeResolver
{
    public static HttpStatusCode Resolve(FlitchException exception)
    {
        return exception switch
        {
            AlreadyExistsException => HttpStatusCode.Conflict,
            InvalidIdentifierException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            RestrictedException => HttpStatusCode.Forbidden,
            ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.NotImplemented,
            
        };
    }
}