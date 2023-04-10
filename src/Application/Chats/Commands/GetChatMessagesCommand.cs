namespace Application.Chats.Commands;

public class GetChatMessagesCommand
{
    public GetChatMessagesCommand(Guid id, int amount, Guid? before)
    {
        Id = id;
        Amount = amount;
        Before = before;
    }
    
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public Guid? Before { get; set; }
    
}