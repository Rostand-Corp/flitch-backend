using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Auth;

public class ForgotPassRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}