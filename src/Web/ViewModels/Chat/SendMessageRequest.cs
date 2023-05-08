using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Chat;

public class SendMessageRequest
{
    [Required(ErrorMessage = "The message content is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "The message content must be between 1 and 500 characters.")]
    public string Content { get; set; }
}