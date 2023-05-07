using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Chat;

public class AddUserToGroupChatRequest
{
    /// <summary>
    /// Id of the user to be added.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
}