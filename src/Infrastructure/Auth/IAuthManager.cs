using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Auth;

public interface IAuthManager
{
    public Task<IdentityUser> RegisterUser(string username, string email, string password);

    public Task<IdentityUser> Login(string username, string password);

    public Task ResetPassword(string email);

    public Task ConfirmEmail(string userId, string token);

    public Task ResendEmailConfirmation(string userId);

    public Task<IEnumerable<Claim>> RetrieveClaims(IdentityUser user);

}