using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Chat.ViewModels;

public class GetMyChatsRequest
{   
    [Range(1, 100, ErrorMessage = "The amount must be between 1 and 100.")]
    public int Amount { get; set; }
    // + type
}