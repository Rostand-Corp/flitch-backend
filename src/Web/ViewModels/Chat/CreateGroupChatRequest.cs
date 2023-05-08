namespace Web.ViewModels.Chat;

public class CreateGroupChatRequest
{
    public IEnumerable<Guid> UserIds { get; set; }
}