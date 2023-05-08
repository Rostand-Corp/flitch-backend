namespace Application.Chats.Commands;

public class AddUserToGroupChatCommand
{
    public AddUserToGroupChatCommand(Guid chatId, Guid userId)
    {
        ChatId = chatId;
        UserId = userId;
    }
    
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
}