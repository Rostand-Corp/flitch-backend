using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

public class ForgotPassRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}