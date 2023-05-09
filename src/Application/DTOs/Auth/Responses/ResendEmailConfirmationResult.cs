namespace Application.DTOs.Auth.Responses;

public abstract class ResendEmailConfirmationResult
{
    private ResendEmailConfirmationResult()
    {
    }

    public class Success : ResendEmailConfirmationResult
    {
        public string Message { get; init; } = "Confirmation e-mail has been resent";
    }

    public class UserDoesntExist : ResendEmailConfirmationResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class EmailAlreadyConfirmed : ResendEmailConfirmationResult
    {
        public string ErrorMessage { get; init; } = "e-mail is already confirmed";
    }

    public class ValidationError : ResendEmailConfirmationResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}