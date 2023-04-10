namespace Web.Controllers.Messenger.Chat.ViewModels;

public class CreateGroupChatRequest
{
    public IEnumerable<Guid> UserIds { get; set; }
}