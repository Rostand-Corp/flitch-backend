namespace Application.DTOs.Chat.Commands;

public class GetChatMessagesCommand
{
    public GetChatMessagesCommand(Guid id, int pageNumber, int amount, string? searchWord, Guid? before)
    {
        Id = id;
        PageNumber = pageNumber;
        Amount = amount;
        SearchWord = searchWord;
        Before = before;
    }
    
    public Guid Id { get; set; }
    public int PageNumber { get; set; }
    public int Amount { get; set; }
    public string? SearchWord { get; set; }
    public Guid? Before { get; set; }
    
}