using System.Security.Authentication;
using System.Security.Claims;
using System.Web;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Auth;

public class AuthManager : IAuthManager
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthManager> _logger;

    public AuthManager(UserManager<IdentityUser> userManager, IEmailSender emailSender, ILogger<AuthManager> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<IdentityUser> RegisterUser(string username, string email, string password)
    {
        var userExists = await _userManager.FindByNameAsync(username) is not null;
        if (userExists)
        {
            _logger.LogWarning("Could not create the user because the user with such nickname already exists");
            throw new AuthenticationException(); // Tf is this type?
        }

        IdentityUser user = new()
        {
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = username
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Could not create the user because of one or more errors occurred");
            throw new AuthenticationException(result.ToString()); // we'll change this too
        }

        await SendConfirmationEmail(user, email);
        _logger.LogInformation("User {UserName} has been successfully created", username);
        return user;
    }

    private async Task SendConfirmationEmail(IdentityUser user, string? email = null)
    {
        if (user is null)
        {
            _logger.LogWarning("Confirmation e-mail could not be sent because specified user value is null (Probably does not exist");
            throw new ArgumentNullException(nameof(user));
        }
        
        var userEmail = email ?? user.Email;
        // Validate once more?

        var emailToken = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user)); // Maybe base64?
        await _emailSender.SendEmailAsync(userEmail, "e-Mail confirmation : Flitch",
            $"Please confirm your email: {emailToken}"); // Atm I don't attach it to an anchor for the sake of testability
        _logger.LogInformation("Confirmation e-mail has been queued to {UserName} ({Email})",
            user.Email, user.UserName);
    }
    
    private async Task SendConfirmationEmail(string userId, string? email = null)
    {
        if(string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Confirmation e-mail could not be sent because specified user Id value is null or empty");
            throw new ArgumentException("Must not be null or empty", nameof(userId));
        }
        var user = await _userManager.FindByIdAsync(userId);
        
        await SendConfirmationEmail(user, email);
    }

    public async Task<IdentityUser> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
        {
            _logger.LogWarning("Could not create the user because the user with such nickname does not exist");
            throw new AuthenticationException("NOUSER"); // yep also here
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            _logger.LogWarning("Password supplied for could not be verified for {UserName} ({Email})",
                user.UserName, user.Email);
            throw new AuthenticationException("Wrong pass"); // !
        }

        _logger.LogInformation("User {UserName} has successfully logged in", username);
        return user;
    }

    public async Task ResetPassword(string email, string oldPassword, string newPassword)
    {
        var validationProblems = new List<string>();

        if (string.IsNullOrEmpty(email)) validationProblems.Add("Email must be specified");
        if (string.IsNullOrEmpty(oldPassword)) validationProblems.Add("Old password must be specified");
        if (string.IsNullOrEmpty(newPassword)) validationProblems.Add("New password must be specified");

        if (validationProblems.Count != 0)
        {
            var errorMessage = string.Join("\r\n", validationProblems);
            throw new AuthenticationException(errorMessage);
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) throw new AuthenticationException("cannot reset password cuz there is no such user");

        if (!user.EmailConfirmed) throw new AuthenticationException("email not confirmed");

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded) throw new AuthenticationException(result.ToString());
    }

    public async Task ConfirmEmail(string userId, string emailToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("e-mail couldn't be confirmed because specified user Id value is null or empty");
            throw new ArgumentException("User id must be not be null or empty");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("e-mail couldn't be confirmed because specified user does not exist");
            throw new AuthenticationException("no such user");
        }

        if (user.EmailConfirmed)
        {
            _logger.LogWarning("e-mail is already confirmed for {UserName}", user.UserName);
            throw new AuthenticationException("already confirmed!!!!!!!");
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, emailToken); // It will run its own validations on arguments anyway.
        if (!result.Succeeded)
        {
            _logger.LogWarning("e-mail couldn't be confirmed because of one or more problems occurred");
            throw new AuthenticationException(result.ToString());
        }
    }

    public async Task ResendEmailConfirmationById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) throw new AuthenticationException("doesnt exist");

        await ResendEmailConfirmation(user);
    }

    public async Task
        ResendEmailConfirmationByEmail(
            string email) // Validate for correctness of email structure I guess (are we going ddd?)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) throw new AuthenticationException("doesnt exist");
        await ResendEmailConfirmation(user);
    }

    private async Task ResendEmailConfirmation(IdentityUser user)
    {
        if (user is null)
        {
            _logger.LogWarning("e-mail couldn't be resent because specified user does not exist");
            throw new AuthenticationException("doesn't exist");
        }
        
        if (user.EmailConfirmed)
        {
            _logger.LogWarning("e-mail is already confirmed for {UserName}", user.UserName);
            throw new AuthenticationException("already confirmed!!!!!!!");
        }

        _logger.LogInformation("Resending email confirmation...");
        await SendConfirmationEmail(user);
    }

    public async Task<IEnumerable<Claim>> RetrieveClaims(IdentityUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}