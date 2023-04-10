using Application.Chats.Commands;

namespace Web.Controllers.Messenger.Chat.ViewModels;

public class GetChatByIdRequest
{
    public string Flag { get; set; } // {default, ...} check ChatDisplayFlag
}