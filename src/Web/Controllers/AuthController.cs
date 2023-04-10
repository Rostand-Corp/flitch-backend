using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Auth;
using Infrastructure.Auth.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthManager authManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user in the system using JWT
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A structure representing JWT and its expiry date for authenticated user</returns>
        /// <response code="200">Returns a JWT and its expiry date</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="401">Returns problem details if password is invalid</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("login")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var result = await _authManager.Login(request.Username!, request.Password!);

            return result switch
            {
                LoginResult.Success res => Ok(await GenerateToken(res.User.Id.ToString())), //.Let(t => (t.Token, t.ExpiryDate)
                LoginResult.InvalidPassword res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.InvalidPass"),
                LoginResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 400,
                        title: "Authentication problem", type: "Auth.Validation"),
                LoginResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Registers user in the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A structure representing JWT and its expiry date for registered user</returns>
        /// <response code="200">Returns a JWT and its expiry date</response>
        /// <response code="401">Returns problem details if username is already taken</response>
        /// <response code="401">Returns problem details if e-mail is already taken</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("register")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            var result = await _authManager.RegisterUser(request.Username!, request.FullName!,request.Email!, request.Password!);

            return result switch
            {
                RegistrationResult.Success res => Ok(
                    await GenerateToken(res.User.Id.ToString())), //.Let(t => (t.Token, t.ExpiryDate)
                RegistrationResult.UserNameTaken res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NameTaken"),
                RegistrationResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                RegistrationResult.EmailTaken res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.EmailTaken"),
                _ => throw new NotImplementedException()
            };

        }

        /// <summary>
        /// Confirms user's e-mail
        /// </summary>
        /// <param name="token"></param>
        /// <returns>A message verifying successful confirmation</returns>
        /// <response code="200">Returns a JWT and its expiry date</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="401">Returns problem details if e-mail is already confirmed</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpPost]
        [Route("confirm-email")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult>
            ConfirmEmail(
                [Required] string token) // Well, I wonder if I can just fire and forget this. Need to resolve disposal problems then
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            var result = await _authManager.ConfirmEmail(userId, token);

            return result switch
            {
                EmailConfirmationResult.Success res => Ok(res.Message),
                EmailConfirmationResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                EmailConfirmationResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                EmailConfirmationResult.EmailAlreadyConfirmed res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.EmailAlreadyConfirmed"),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Resends e-mail confirmation message to the user
        /// </summary>
        /// <returns>A message verifying that e-mail has been resent</returns>
        /// <response code="200">Returns a message verifying e-mail has been resent</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="401">Returns problem details if e-mail is already confirmed</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpGet]
        [Route("resend-confirmation")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // Validate
            var result = await _authManager.ResendEmailConfirmationByEmail(userEmail);

            return result switch
            {
                ResendEmailConfirmationResult.Success res => Ok(res.Message),
                ResendEmailConfirmationResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                ResendEmailConfirmationResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                ResendEmailConfirmationResult.EmailAlreadyConfirmed res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.EmailAlreadyConfirmed"),
                _ => throw new NotImplementedException()
            };

        }
        /// <summary>
        /// Resets user's password by using his current password
        /// </summary>
        /// <returns>A message verifying that password has been reset successfully</returns>
        /// <response code="200">Returns a message verifying that password has been reset successfully</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpPost]
        [Route("reset-password")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassModel request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            var result = await _authManager.ResetPassword(userId!, request.OldPassword!, request.NewPassword!);

            return result switch
            {
                ResetKnownPasswordResult.Success res => Ok(res.Message),
                ResetKnownPasswordResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                ResetKnownPasswordResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                _ => throw new NotImplementedException()
            };
        }
        
        /// <summary>
        /// Sends forgot password e-mail to the user
        /// </summary>
        /// <returns>A message verifying that e-mail has been sent successfully</returns>
        /// <response code="200">Returns a message verifying that e-mail has been sent successfully</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="400">Returns problem details if user's e-mail is not confirmed</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("forgot-password")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> SendForgotPasswordResetEmail([FromBody] ForgotPassModel request)
        {
            var result = await _authManager.SendForgotPasswordResetEmail(request.Email!);

            return result switch
            {
                SendForgotPasswordResetEmailResult.Success res => Ok(res.Message),
                SendForgotPasswordResetEmailResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                SendForgotPasswordResetEmailResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                SendForgotPasswordResetEmailResult.EmailNotConfirmed res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.EmailNotConfirmed"),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Resets user's password by using a reset token from e-mail
        /// </summary>
        /// <returns>A message verifying password has been reset successfully</returns>
        /// <response code="200">Returns a message verifying password has been reset successfully</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="400">Returns problem details if user's e-mail is not confirmed</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("reset-forgot-password")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetForgotPassword([FromBody] ResetForgotPassModel request)
        {
            var result = await _authManager.ResetForgotPassword(request.Email!, request.Token!, request.Password!);

            return result switch
            {
                ResetForgotPasswordResult.Success res => Ok(res.Message),
                ResetForgotPasswordResult.UserDoesntExist res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.NoUser"),
                ResetForgotPasswordResult.ValidationError res =>
                    Problem(ErrorMessagesToString(res.ErrorMessages), statusCode: 401,
                        title: "Authentication problem", type: "Auth.Validation"),
                ResetForgotPasswordResult.EmailNotConfirmed res =>
                    Problem(res.ErrorMessage, statusCode: 401, title: "Authentication problem",
                        type: "Auth.EmailNotConfirmed"),
                _ => throw new NotImplementedException()
            };
        }
        
        [Authorize]
        [HttpGet]
        [Route("test")]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public IActionResult Test()
        {
            return Ok(
            $"""
            You're authorized.
            Your id is {User.Claims.FirstOrDefault(c=>c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Undefined"}
            Your name is {User.Identity?.Name ?? "Undefined"}
            """);
        }

        private async Task<JwtResult> GenerateToken(string userId)
        {
            var claims = await _authManager.RetrieveClaims(userId);
            
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3), // Todo: Not suitable for production. Reduce to 3 minutes
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiryDate = token.ValidTo
            };
        }

        private static string ErrorMessagesToString(IEnumerable<string> messages)
        {
            return string.Join("\r\n", messages);
        }
    }
    
    public class JwtResult
    {
        [Required] public string Token { get; set; }
        [Required] public DateTime ExpiryDate { get; set; }
    }

    public class RegisterModel
    {
        [Required] // Todo: Try moving it to the natural modifier
        [MinLength(6), MaxLength(16)]
        public string? Username { get; set; }
        [Required]
        [MinLength(1), MaxLength(128)]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }
    
    public class LoginModel
    {
        [Required] // Todo: Try moving it to the natural modifier
        [MinLength(6), MaxLength(16)]
        public string? Username { get; set; }
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }

    public class ForgotPassModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class ResetPassModel
    {
        [Required] [MinLength(6)] public string? OldPassword { get; set; }
        [Required] [MinLength(6)] public string? NewPassword { get; set; }
    }

    public class ResetForgotPassModel
    {
        [Required] public string? Password { get; set; }
        [Required] [EmailAddress] public string? Email { get; set; }
        [Required] public string? Token { get; set; }
    }

    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
