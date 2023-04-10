namespace Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    public ChatType Type { get; set; }
    
 // public Guid? CreatorId { get; set; }
 // public ChatUser? Creator { get; set; }
    
    public List<Message> Messages { get; set; } = new();

    public Guid? LastMessageId { get; set; }
    public Message? LastMessage { get; set; }
    public DateTime Created { get; set; }

    public List<User> Users { get; init; } = new();
    public List<ChatUser> Participants { get; init; } = new();

}

public enum ChatType
{
    Private,
    Group
    
}