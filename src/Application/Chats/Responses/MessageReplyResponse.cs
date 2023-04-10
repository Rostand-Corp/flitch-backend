namespace Application.Chats.Responses;

public class MessageReplyResponse
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}