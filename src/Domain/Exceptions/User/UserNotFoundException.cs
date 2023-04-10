using System.Net;

namespace Domain.Exceptions.User;

public class UserNotFoundException : FlitchException
{
    public UserNotFoundException() : base("User.NotFound", "The specified user was not found", HttpStatusCode.NotFound)
    {
    }
}