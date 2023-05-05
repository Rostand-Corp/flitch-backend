using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

public class ResetForgotPassRequest
{
    [Required] 
    public string? Password { get; set; }
    [Required] 
    [EmailAddress] 
    public string? Email { get; set; }
    [Required] public string? Token { get; set; }
}