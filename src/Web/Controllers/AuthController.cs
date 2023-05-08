using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.AppServices.Auth;
using Application.DTOs.Auth.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Web.ViewModels.Auth;

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
        [ProducesResponseType(typeof(JwtResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authManager.Login(request.Email!, request.Password!);

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
        [ProducesResponseType(typeof(JwtResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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
        /// <returns>Returns Ok</returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="401">Returns problem details if e-mail is already confirmed</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpPost]
        [Route("confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult>
            ConfirmEmail(
                [Required] string token) // Well, I wonder if I can just fire and forget this. Need to resolve disposal problems then
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            var result = await _authManager.ConfirmEmail(userId, token);

            return result switch
            {
                EmailConfirmationResult.Success res => Ok(),
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
        /// <returns>Returns Ok</returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="401">Returns problem details if e-mail is already confirmed</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpGet]
        [Route("resend-confirmation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // Validate
            var result = await _authManager.ResendEmailConfirmationByEmail(userEmail);

            return result switch
            {
                ResendEmailConfirmationResult.Success res => Ok(),
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
        /// <returns>Returns Ok</returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize]
        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            var result = await _authManager.ResetPassword(userId!, request.OldPassword!, request.NewPassword!);

            return result switch
            {
                ResetKnownPasswordResult.Success res => Ok(),
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
        /// <returns>Returns Ok</returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="400">Returns problem details if user's e-mail is not confirmed</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> SendForgotPasswordResetEmail([FromBody] ForgotPassRequest request)
        {
            var result = await _authManager.SendForgotPasswordResetEmail(request.Email!);

            return result switch
            {
                SendForgotPasswordResetEmailResult.Success res => Ok(),
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
        /// <returns>Returns Ok</returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="401">Returns problem details if user does not exist</response>
        /// <response code="400">Returns problem details if validation problem occurred</response>
        /// <response code="400">Returns problem details if user's e-mail is not confirmed</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpPost]
        [Route("reset-forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> ResetForgotPassword([FromBody] ResetForgotPassRequest request)
        {
            var result = await _authManager.ResetForgotPassword(request.Email!, request.Token!, request.Password!);

            return result switch
            {
                ResetForgotPasswordResult.Success res => Ok(),
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
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
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
    
}
