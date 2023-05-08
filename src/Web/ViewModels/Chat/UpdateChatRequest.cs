using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Chat;

public class UpdateChatRequest
{
    /// <summary>
    /// A name to change chat's current name for (1-50). Name cannot be changed for private chats.  
    /// </summary>
    [Required]
    [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "The chat name must be between 1 and 50 characters.")]
    public string Name { get; set; }
}