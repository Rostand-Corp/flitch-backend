using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Auth;

public interface IAuthManager
{
    public Task<IdentityUser> RegisterUser(string username, string email, string password);

    public Task<IdentityUser> Login(string username, string password);

    public Task ResetPassword(string email, string oldPassword, string newPassword);

    public Task SendForgotPasswordResetEmail(string email);

    public Task ResetForgotPassword(string email, string token, string newPassword);

    public Task ConfirmEmail(string userId, string token);

    public Task ResendEmailConfirmationById(string userId);

    public Task ResendEmailConfirmationByEmail(string email);

    public Task<IEnumerable<Claim>> RetrieveClaims(IdentityUser user);

}