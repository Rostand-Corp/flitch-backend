using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

public class ForgotPassModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}