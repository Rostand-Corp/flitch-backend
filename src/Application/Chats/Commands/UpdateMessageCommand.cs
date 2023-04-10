namespace Application.Chats.Commands;

public class UpdateMessageCommand
{
    public UpdateMessageCommand(Guid chatId, Guid messageId, string content)
    {
        ChatId = chatId;
        MessageId = messageId;
        Content = content;
    }

    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
    public string Content { get; set; }
}