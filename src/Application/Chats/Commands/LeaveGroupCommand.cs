namespace Application.Chats.Commands;

public class LeaveGroupCommand
{
    public LeaveGroupCommand(Guid chatId)
    {
        ChatId = chatId;
    }
    
    public Guid ChatId { get; set; }
}