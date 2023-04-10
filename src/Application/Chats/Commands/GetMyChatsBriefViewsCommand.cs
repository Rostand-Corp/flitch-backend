namespace Application.Chats.Commands;

public class GetMyChatsBriefViewsCommand
{
    public GetMyChatsBriefViewsCommand(int amount)
    {
        Amount = amount;
    }
    
    public int Amount { get; set; }
}