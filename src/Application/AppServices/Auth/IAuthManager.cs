using System.Security.Claims;
using Application.Auth.Results;

namespace Application.Auth;

public interface IAuthManager
{
    public Task<RegistrationResult> RegisterUser(string username, string fullName, string email, string password);

    public Task<LoginResult> Login(string email, string password);

    public Task<ResetKnownPasswordResult> ResetPassword(string email, string oldPassword, string newPassword);

    public Task<SendForgotPasswordResetEmailResult> SendForgotPasswordResetEmail(string email);

    public Task<ResetForgotPasswordResult> ResetForgotPassword(string email, string token, string newPassword);

    public Task<EmailConfirmationResult> ConfirmEmail(string userId, string token);

    public Task<ResendEmailConfirmationResult> ResendEmailConfirmationById(string userId);

    public Task<ResendEmailConfirmationResult> ResendEmailConfirmationByEmail(string email);
    
    public Task<IEnumerable<Claim>> RetrieveClaims(string userId);

}