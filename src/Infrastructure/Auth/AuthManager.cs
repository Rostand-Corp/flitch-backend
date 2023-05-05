using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Web;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Validators;
using Infrastructure.Auth.Results;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Auth;

public class AuthManager : IAuthManager
{
    private readonly UserManager<SystemUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthManager> _logger;
    public AuthManager(
        UserManager<SystemUser> userManager,
        IEmailSender emailSender,
        ILogger<AuthManager> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<RegistrationResult> RegisterUser(string username, string fullName, string email, string password)
    {
        // TODO:
        // I believe it is better to use TransactionScope here though unnecessary. Well, I will once any problem appear.
        
        var userNameTaken = await _userManager.FindByNameAsync(username) is not null;
        if (userNameTaken)
        {
            _logger.LogInformation("Could not create the user because the user with such nickname already exists");
            return new RegistrationResult.UserNameTaken
            {
                ErrorMessage = Resources.UserNameTaken
            };
        }

        var emailNameTaken = await _userManager.FindByEmailAsync(username) is not null;
        if (userNameTaken)
        {
            _logger.LogInformation("Could not create the user because the user with such email already exists");
            return new RegistrationResult.EmailTaken
            {
                ErrorMessage = Resources.EmailTaken
            };
        }
        
        SystemUser user = new()
        {
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = username,
            FullName = fullName
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

        var regResult = await RegisterInSubsystems(user);
        
        if (!regResult.Succeeded)
        {
            _logger.LogInformation("Could not create the user because of one or more errors occurred");
            var rollbackDeleteResult = await _userManager.DeleteAsync(user);
            if (!rollbackDeleteResult.Succeeded)
            {
                throw new Exception(String.Join(" ", rollbackDeleteResult)); // todo: 
            }
            return new RegistrationResult.ValidationError
            {
                ErrorMessages = regResult.Errors.Select(error => error.Description)
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

    private async Task SendConfirmationEmail(SystemUser user, string? email = null)
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
            Resources.EmailConfirmationSubject,
            string.Format(Resources.EmailConfirmationMessage, emailTokenEncoded));
        
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

    public async Task<LoginResult> Login(string email, string password)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(email)) validationMessages.Add(Resources.SpecifyUserName);

        if (string.IsNullOrEmpty(password)) validationMessages.Add(Resources.SpecifyPassword);

        if (validationMessages.Any())
            return new LoginResult.ValidationError
            {
                ErrorMessages = validationMessages
            };

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogInformation("Could not create the user because the user with such email does not exist");
            return new LoginResult.UserDoesntExist
            {
                ErrorMessage = Resources.UserDoesntExist
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
                ErrorMessage = Resources.PasswordInvalid
            };
        }

        _logger.LogInformation("User {UserName} has successfully logged in", user.Email);

        return new LoginResult.Success
        {
            User = user
        };
    }

    public async Task<ResetKnownPasswordResult> ResetPassword(string userId, string oldPassword, string newPassword)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("Must not be null or empty", nameof(userId)); 
        if (string.IsNullOrEmpty(oldPassword)) validationMessages.Add(Resources.SpecifyOldPassword);
        if (string.IsNullOrEmpty(newPassword)) validationMessages.Add(Resources.SpecifyNewPassword);

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
                ErrorMessage = Resources.UserDoesntExist
            };

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
            return new ResetKnownPasswordResult.ValidationError
            {
                ErrorMessages = result.Errors.Select(error => error.Description)
            };

        return new ResetKnownPasswordResult.Success
        {
            Message = Resources.PasswordChangedSuccess
        };
    }

    public async Task<SendForgotPasswordResetEmailResult> SendForgotPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return new SendForgotPasswordResetEmailResult.ValidationError
            {
                ErrorMessages = new List<string> {Resources.SpecifyEmail}
            };

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return new SendForgotPasswordResetEmailResult.UserDoesntExist
            {
                ErrorMessage = Resources.UserDoesntExist
            };

        if (!user.EmailConfirmed)
            return new SendForgotPasswordResetEmailResult.EmailNotConfirmed
            {
                ErrorMessage = Resources.EmailWasNotConfirmed
            };

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetTokenEncoded = HttpUtility.UrlEncode(resetToken);
        
        await _emailSender.SendEmailAsync(
            email,
            Resources.EmailPasswordResetSubject,
            string.Format(Resources.EmailPasswordResetMessage, resetTokenEncoded));
        
        _logger.LogInformation(
            "Password reset e-mail has been queued to {UserName} ({Email})",
            user.UserName,
            user.Email);

        return new SendForgotPasswordResetEmailResult.Success
        {
            Message = Resources.EmailPasswordResetMessageSent
        };
    }

    public async Task<ResetForgotPasswordResult>
        ResetForgotPassword(string email, string token, string newPassword)
    {
        var validationMessages = new List<string>();

        if (string.IsNullOrEmpty(email)) validationMessages.Add(Resources.SpecifyEmail);
        if (string.IsNullOrEmpty(token)) validationMessages.Add(Resources.SpecifyResetToken);
        if (string.IsNullOrEmpty(newPassword)) validationMessages.Add(Resources.SpecifyNewPassword);

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
                ErrorMessage = Resources.UserDoesntExist
            };

        if (!user.EmailConfirmed)
            return new ResetForgotPasswordResult.EmailNotConfirmed
            {
                ErrorMessage = Resources.EmailWasNotConfirmed
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
            Message = Resources.PasswordResetSuccess
        };
    }

    public async Task<EmailConfirmationResult> ConfirmEmail(string userId, string emailToken)
    {
        var validationMessages = new List<string>();
        
        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("Must not be null or empty", nameof(emailToken));
        if (string.IsNullOrEmpty(emailToken)) validationMessages.Add(Resources.SpecifyEmailConfirmationToken);
        
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
                ErrorMessage = Resources.UserDoesntExist
            };
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("e-mail is already confirmed for {UserName}", user.UserName);
            return new EmailConfirmationResult.EmailAlreadyConfirmed
            {
                ErrorMessage = Resources.EmailAlreadyConfirmed
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
            Message = Resources.EmailConfirmationSuccess
        };
    }

    public async Task<ResendEmailConfirmationResult> ResendEmailConfirmationById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new ResendEmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = Resources.UserDoesntExist
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
                ErrorMessage = Resources.UserDoesntExist
            };
        return await ResendEmailConfirmation(user);
    }

    private async Task<ResendEmailConfirmationResult> ResendEmailConfirmation(SystemUser user)
    {
        if (user is null)
        {
            _logger.LogInformation("e-mail couldn't be resent because specified user does not exist");
            return new ResendEmailConfirmationResult.UserDoesntExist
            {
                ErrorMessage = Resources.UserDoesntExist
            };
        }
        
        if (user.EmailConfirmed)
        {
            _logger.LogInformation("e-mail is already confirmed for {UserName}", user.UserName);
            return new ResendEmailConfirmationResult.EmailAlreadyConfirmed
            {
                ErrorMessage = Resources.EmailAlreadyConfirmed
            };
        }

        _logger.LogInformation("Resending email confirmation...");
        await SendConfirmationEmail(user);
        return new ResendEmailConfirmationResult.Success
        {
            Message = Resources.EmailConfirmationResendSuccess
        };
    }

    private async Task<IdentityResult> RegisterInSubsystems(SystemUser user)
    {
        var messengerUser = new User()
        {
            DisplayName = user.UserName,
            FullName = user.FullName,
            Status = "Hi, I'm new here!",
        };
        
        var validationResult = new UserValidator().Validate(messengerUser); // inject validator?
        if (!validationResult.IsValid)
        {
            var rollbackDeleteResult = await _userManager.DeleteAsync(user);
            if (!rollbackDeleteResult.Succeeded)
            {
                throw new Exception(String.Join(" ", rollbackDeleteResult));// todo: handle this properly
            }
            throw new ValidationException("User", validationResult.ToDictionary()); // exception / result.
        }

        user.MessengerUser = messengerUser;

        return await _userManager.UpdateAsync(user);

    }

    public async Task<IEnumerable<Claim>> RetrieveClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
        };
        AddSubsystemsIdentityClaims(user, claims);

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;

        void AddSubsystemsIdentityClaims(SystemUser user, List<Claim> claims)
        {
            // TODO: Refactor to add all the claims
            if (user.MessengerUserId is not null)
                claims.Add(new Claim("msngrUserId", user.MessengerUserId.ToString()));
        }
    }
}