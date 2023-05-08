namespace Application.Auth.Results;

public abstract class ResetForgotPasswordResult
{
    private ResetForgotPasswordResult()
    {
    }

    public class Success : ResetForgotPasswordResult
    {
        public string Message { get; init; } = "Password has been successfully reset";
    }

    public class UserDoesntExist : ResetForgotPasswordResult
    {
        public string ErrorMessage { get; init; } = "User doesn't exist";
    }

    public class EmailNotConfirmed : ResetForgotPasswordResult
    {
        public string ErrorMessage { get; init; } = "e-mail was not confirmed by the user";
    }

    public class ValidationError : ResetForgotPasswordResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}