using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Chat.ViewModels;

public class AddUserToGroupChatRequest
{
    /// <summary>
    /// Id of the user to be added.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
}