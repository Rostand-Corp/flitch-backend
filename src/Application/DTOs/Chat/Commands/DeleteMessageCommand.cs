namespace Application.DTOs.Chat.Commands;

public class DeleteMessageCommand
{
    public DeleteMessageCommand(Guid chatId, Guid messageId)
    {
        ChatId = chatId;
        MessageId = messageId;
    }
    
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
}