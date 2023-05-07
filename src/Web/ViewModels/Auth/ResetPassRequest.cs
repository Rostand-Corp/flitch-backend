using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Auth;

public class ResetPassRequest
{
    [Required] 
    [MinLength(6)] 
    public string? OldPassword { get; set; }
    [Required] 
    [MinLength(6)] 
    public string? NewPassword { get; set; }
}