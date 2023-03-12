using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Auth.Results;

public abstract class LoginResult
{
    private LoginResult()
    {
    }

    public class Success : LoginResult
    {
        public IdentityUser User { get; init; }
    }

    public class UserDoesntExist : LoginResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class InvalidPassword : LoginResult
    {
        public string ErrorMessage { get; init; } = "Provided password is invalid";
    }

    public class ValidationError : LoginResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}