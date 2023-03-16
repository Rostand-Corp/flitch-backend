using System.Security.Claims;
using System.Web;
using Infrastructure.Auth.Results;
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

    public async Task<RegistrationResult> RegisterUser(string username, string email, string password)
    {
        var userNameTaken = await _userManager.FindByNameAsync(username) is not null;
        if (userNameTaken)
        {
            _logger.LogInformation("Could not create the user because the user with such nickname already exists");
            return new RegistrationResult.UserNameTaken
            {
                ErrorMessage = "User already exists"
            };
        }

        var emailNameTaken = await _userManager.FindByEmailAsync(username) is not null;
        if (userNameTaken)
        {
            _logger.LogInformation("Could not create the user because the user with such email already exists");
            return new RegistrationResult.EmailTaken
            {
                ErrorMessage = "User already exists"
            };
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
            _logger.LogInformation("Could not create the user because of one or more errors occurred");
            return new RegistrationResult.ValidationError
            {
                ErrorMessages = result.Errors.Select(error => error.Description)
            };
        }

        await SendConfirmationEmail(user, email);
        _logger.LogInformation(
            "User {UserName} has been successfully created",
            username);
        return new RegistrationResult.Success
        {
            User = user
        };
    }

    private async Task SendConfirmationEmail(IdentityUser user, string? email = null)
    {
        if (user is null)
        {
            _logger.LogWarning(
                "Confirmation e-mail could not be sent because specified user value is null (Probably does not exist");
            throw new ArgumentNullException(nameof(user));
        }
        
        var userEmail = email ?? user.Email;

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailTokenEncoded = HttpUtility.UrlEncode(emailToken);
        await _emailSender.SendEmailAsync(
            userEmail,
            "e-Mail confirmation : Flitch",
            $"Please confirm your email: {emailTokenEncoded}");
        
        _logger.LogInformation(
            "Confirmation e-mail has been queued to {UserName} ({Email})",
            user.Email,
            user.UserName);
    }
    
    private async Task SendConfirmationEmail(string userId, string? email = null)
    {
        if(string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation(
                "Confirmation e-mail could not be sent because specified user Id value is null or empty");
            throw new ArgumentException("Must not be null or empty", nameof(userId));
        }
        var user = await _userManager.FindByIdAsync(userId);
        
        await SendConfirmationEmail(user, email);
    }

    public async Task<LoginResult> Login(string username, string password)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(username)) validationMessages.Add("Username must be specified");

        if (string.IsNullOrEmpty(password)) validationMessages.Add("Password must be specified");

        if (validationMessages.Any())
            return new LoginResult.ValidationError
            {
                ErrorMessages = validationMessages
            };

        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
        {
            _logger.LogInformation("Could not create the user because the user with such nickname does not exist");
            return new LoginResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            _logger.LogInformation(
                "The password supplied could not be verified for {UserName} ({Email})",
                user.UserName,
                user.Email);
            return new LoginResult.InvalidPassword
            {
                ErrorMessage = "Provided password is invalid"
            };
        }

        _logger.LogInformation("User {UserName} has successfully logged in", username);

        return new LoginResult.Success
        {
            User = user
        };
    }

    public async Task<ResetKnownPasswordResult> ResetPassword(string userId, string oldPassword, string newPassword)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(userId)) validationMessages.Add("UserId must be specified");
        if (string.IsNullOrEmpty(oldPassword)) validationMessages.Add("Old password must be specified");
        if (string.IsNullOrEmpty(newPassword)) validationMessages.Add("New password must be specified");

        if (validationMessages.Any())
        {
            return new ResetKnownPasswordResult.ValidationError
            {
                ErrorMessages = validationMessages
            };
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return new ResetKnownPasswordResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
            return new ResetKnownPasswordResult.ValidationError
            {
                ErrorMessages = result.Errors.Select(error => error.Description)
            };

        return new ResetKnownPasswordResult.Success
        {
            Message = "Password has been successfully changed"
        };
    }

    public async Task<SendForgotPasswordResetEmailResult> SendForgotPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return new SendForgotPasswordResetEmailResult.ValidationError
            {
                ErrorMessages = new List<string> {"e-mail must not be null or empty"}
            };

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return new SendForgotPasswordResetEmailResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };

        if (!user.EmailConfirmed)
            return new SendForgotPasswordResetEmailResult.EmailNotConfirmed
            {
                ErrorMessage = "e-mail was not confirmed by the user"
            };

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetTokenEncoded = HttpUtility.UrlEncode(resetToken);
        
        await _emailSender.SendEmailAsync(
            email,
            "Password reset : Flitch",
            $"Follow this link to reset your password: {resetTokenEncoded}");
        
        _logger.LogInformation(
            "Password reset e-mail has been queued to {UserName} ({Email})",
            user.UserName,
            user.Email);

        return new SendForgotPasswordResetEmailResult.Success
        {
            Message =
                "Password reset email has been queued. If you cannot see an email then consider requesting new message"
        };
    }

    public async Task<ResetForgotPasswordResult>
        ResetForgotPassword(string email, string token, string newPassword)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(email)) validationMessages.Add("e-mail must be specified");
        if (string.IsNullOrEmpty(token)) validationMessages.Add("Token must be specified");
        if (string.IsNullOrEmpty(newPassword)) validationMessages.Add("New password must be specified");

        if (validationMessages.Any())
        {
            return new ResetForgotPasswordResult.ValidationError
            {
                ErrorMessages = validationMessages
            };
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return new ResetForgotPasswordResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };

        if (!user.EmailConfirmed)
            return new ResetForgotPasswordResult.EmailNotConfirmed
            {
                ErrorMessage = "e-mail was not confirmed by the user"
            };

        var resetTokenDecoded = HttpUtility.UrlDecode(token);
        var result = await _userManager.ResetPasswordAsync(user, resetTokenDecoded, newPassword);

        if (!result.Succeeded)
            return new ResetForgotPasswordResult.ValidationError
            {
                ErrorMessages = result.Errors.Select(error => error.Description)
            };
        _logger.LogInformation(
            "Password has been reset for {UserName} ({Email})",
            user.UserName,
            user.Email);

        return new ResetForgotPasswordResult.Success
        {
            Message = "Password has been successfully reset"
        };
    }

    public async Task<EmailConfirmationResult> ConfirmEmail(string userId, string emailToken)
    {
        var validationMessages = new List<string>();
        
        if (string.IsNullOrEmpty(userId))
        {
            validationMessages.Add("User id must be not be null or empty");
        }

        if (validationMessages.Any())
            return new EmailConfirmationResult.ValidationError
            {
                ErrorMessages = validationMessages
            };

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new EmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("e-mail is already confirmed for {UserName}", user.UserName);
            return new EmailConfirmationResult.EmailAlreadyConfirmed
            {
                ErrorMessage = "Email is already confirmed"
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, emailToken);
        if (!result.Succeeded)
        {
            _logger.LogInformation("e-mail couldn't be confirmed because of one or more problems occurred");
            return new EmailConfirmationResult.ValidationError
            {
                ErrorMessages = result.Errors.Select(error => error.Description)
            };
        }

        return new EmailConfirmationResult.Success
        {
            Message = "Email has been successfully confirmed"
        };
    }

    public async Task<ResendEmailConfirmationResult> ResendEmailConfirmationById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new ResendEmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };

        return await ResendEmailConfirmation(user);
    }

    public async Task<ResendEmailConfirmationResult>
        ResendEmailConfirmationByEmail(
            string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return new ResendEmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };
        return await ResendEmailConfirmation(user);
    }

    private async Task<ResendEmailConfirmationResult> ResendEmailConfirmation(IdentityUser user)
    {
        if (user is null)
        {
            _logger.LogInformation("e-mail couldn't be resent because specified user does not exist");
            return new ResendEmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = "User doesn't exist"
            };
        }
        
        if (user.EmailConfirmed)
        {
            _logger.LogInformation("e-mail is already confirmed for {UserName}", user.UserName);
            return new ResendEmailConfirmationResult.EmailAlreadyConfirmed
            {
                ErrorMessage = "e-mail is already confirmed"
            };
        }

        _logger.LogInformation("Resending email confirmation...");
        await SendConfirmationEmail(user);
        return new ResendEmailConfirmationResult.Success
        {
            Message = "Confirmation e-mail has been resent"
        };
    }

    public async Task<IEnumerable<Claim>> RetrieveClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
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