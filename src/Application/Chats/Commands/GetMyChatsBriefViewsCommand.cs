namespace Application.Chats.Commands;

public class GetMyChatsBriefViewsCommand
{
    public GetMyChatsBriefViewsCommand(int pageNumber, int amount, string searchWord)
    {
        PageNumber = pageNumber;
        Amount = amount;
        SearchWord = searchWord;
    }
    
    public int PageNumber { get; set; }
    public int Amount { get; set; }
    public string? SearchWord { get; set; }
}