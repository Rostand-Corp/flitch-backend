namespace Application.Auth.Results;

public abstract class SendForgotPasswordResetEmailResult
{
    private SendForgotPasswordResetEmailResult()
    {
    }

    public class Success : SendForgotPasswordResetEmailResult
    {
        public string Message { get; init; } =
            "Password reset email has been queued. If you cannot see an email then consider requesting new message";
    }

    public class UserDoesntExist : SendForgotPasswordResetEmailResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class EmailNotConfirmed : SendForgotPasswordResetEmailResult
    {
        public string ErrorMessage { get; init; } = "e-mail was not confirmed by the user";
    }

    public class ValidationError : SendForgotPasswordResetEmailResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}