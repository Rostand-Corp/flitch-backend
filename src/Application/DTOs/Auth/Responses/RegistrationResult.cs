using Infrastructure.Data;

namespace Application.DTOs.Auth.Responses;

public abstract class RegistrationResult
{
    private RegistrationResult()
    {
    }

    public class Success : RegistrationResult
    {
        public SystemUser User { get; init; }
    }

    public class UserNameTaken : RegistrationResult
    {
        public string ErrorMessage { get; init; } = "Username is already taken";
    }

    public class EmailTaken : RegistrationResult
    {
        public string ErrorMessage { get; init; } = "Email is already taken";
    }

    public class ValidationError : RegistrationResult
    {
        public IEnumerable<string> ErrorMessages { get; init; } =
            new List<string> {"One or more validation errors occurred"};
    }
}