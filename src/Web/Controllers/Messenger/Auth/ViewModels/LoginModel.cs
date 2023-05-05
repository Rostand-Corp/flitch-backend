using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

public class LoginModel
{
    [Required] // Todo: Try moving it to the natural modifier
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string? Password { get; set; }
}