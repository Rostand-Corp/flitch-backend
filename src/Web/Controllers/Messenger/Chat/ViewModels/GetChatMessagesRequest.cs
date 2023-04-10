using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Chat.ViewModels;

public class GetChatMessagesRequest
{
    [Range(1, 100, ErrorMessage = "The amount must be between 1 and 100.")]
    public int Amount { get; set; }
    public Guid? Before { get; set; }
}