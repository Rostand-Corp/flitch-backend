namespace Application.DTOs.Chat.Commands;

public class GetMyChatsBriefViewsCommand
{
    public GetMyChatsBriefViewsCommand(int pageNumber, int amount, ChatTypeFilter filter)
    {
        PageNumber = pageNumber;
        Amount = amount;
        Filter = filter;
    }
    
    public int PageNumber { get; set; }
    public int Amount { get; set; }
    public ChatTypeFilter Filter { get; set; }
}

public enum ChatTypeFilter
{
    All,
    Private
}