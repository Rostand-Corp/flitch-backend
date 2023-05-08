namespace Application.Auth.Results;

public abstract class EmailConfirmationResult
{
    private EmailConfirmationResult()
    {
    }

    public class Success : EmailConfirmationResult
    {
        public string Message { get; init; } = "Email has been successfully confirmed";
    }

    public class UserDoesntExist : EmailConfirmationResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class EmailAlreadyConfirmed : EmailConfirmationResult
    {
        public string ErrorMessage { get; init; } = "Email is already confirmed";
    }

    public class ValidationError : EmailConfirmationResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}