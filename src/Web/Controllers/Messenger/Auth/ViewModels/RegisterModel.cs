﻿using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Auth.ViewModels;

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