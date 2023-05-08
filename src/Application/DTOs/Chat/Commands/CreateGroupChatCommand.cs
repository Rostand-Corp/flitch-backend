namespace Application.Chats.Commands;

public class CreateGroupChatCommand
{
    public CreateGroupChatCommand(IEnumerable<Guid> userIds)
    {
        UserIds = userIds;
    }
    
    public IEnumerable<Guid> UserIds { get; set; } // Usernames??
}