using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Auth;
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var user = await _authManager.Login(request.Username!, request.Password!);

            var claims = await _authManager.RetrieveClaims(user);
            var token = GenerateToken(claims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            var newUser = await _authManager.RegisterUser(request.Username!, request.Email!, request.Password!);

            var claims = await _authManager.RetrieveClaims(newUser);
            var token = GenerateToken(claims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration =  token.ValidTo
            });
        }

        [Authorize]
        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token) // Well, I wonder if I can just fire and forget this. Need to resolve disposal problems then
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            await _authManager.ConfirmEmail(userId, token);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("resend-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // Validate
            await _authManager.ResendEmailConfirmationByEmail(userEmail);
            return Ok();

        }

        [Authorize]
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassModel request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // Validate
            await _authManager.ResetPassword(userId!, request.OldPassword!, request.NewPassword!);
            return Ok();
        }

        [HttpPost]
        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return Ok();
        }
        

        [Authorize]
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok(
            $"""
            You're authorized.
            Your id is {User.Claims.FirstOrDefault(c=>c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Undefined"}
            Your name is {User.Identity?.Name ?? "Undefined"}
            """);
        }
        
        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }

    public class RegisterModel
    {
        [Required] // Todo: Try moving it to the natural modifier
        [MinLength(6), MaxLength(16)]
        public string? Username { get; set; }
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

    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
