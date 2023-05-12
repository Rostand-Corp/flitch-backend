namespace Application.DTOs.Auth.Responses;

public abstract class ResetKnownPasswordResult
{
    private ResetKnownPasswordResult()
    {
    }

    public class Success : ResetKnownPasswordResult
    {
        public string Message { get; init; } = "Password has been successfully reset";
    }

    public class UserDoesntExist : ResetKnownPasswordResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class ValidationError : ResetKnownPasswordResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}