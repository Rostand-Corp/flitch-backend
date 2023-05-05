﻿using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

public class ResetPassModel
{
    [Required] 
    [MinLength(6)] 
    public string? OldPassword { get; set; }
    [Required] 
    [MinLength(6)] 
    public string? NewPassword { get; set; }
}