namespace Application.Chats.Commands;

public class SendMessageCommand
{
    public SendMessageCommand(Guid chatId, string content)
    {
        ChatId = chatId;
        Content = content;
    }
    
    public Guid ChatId { get; set; }
    public string Content { get; set; } = null!;
}