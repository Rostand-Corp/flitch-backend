using Domain.Entities;

namespace Application.DTOs.Chat.Responses;

public class MessageResponse
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    public ChatUserMinimalNoChatResponse Sender { get; set; }
    
    public MessageReplyResponse InReplyTo { get; set; }

    public MessageType Type { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }

}