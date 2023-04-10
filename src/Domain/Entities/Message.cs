namespace Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    
    public Guid SenderId { get; set; }
    public ChatUser Sender { get; set; }

    public string Content { get; set; }
    public MessageType Type { get; set; }

    public Message? ReplyTo { get; set; }
    
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public bool IsVisible { get; set; }
}

public enum MessageType
{
    Default,
    AdminOnly,
    System
}