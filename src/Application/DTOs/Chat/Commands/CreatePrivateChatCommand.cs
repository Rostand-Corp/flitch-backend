namespace Application.Chats.Commands;

public class CreatePrivateChatCommand
{
    public CreatePrivateChatCommand(Guid userId)
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
    
}