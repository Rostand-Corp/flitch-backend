using System.ComponentModel.DataAnnotations;
namespace Web.Controllers.Messenger.Chat.ViewModels;

public class UpdateMessageRequest
{
    [Required(ErrorMessage = "The message content is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "The message content must be between 1 and 500 characters.")]
    public string Content { get; set; }
}