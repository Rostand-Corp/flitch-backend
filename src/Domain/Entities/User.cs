namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string? Status { get; set; }
    
    public Guid? ColorId { get; set; }
    public Color? Color { get; set; }

    public List<Chat> Chats { get; } = new();
    public List<ChatUser> ChatUsers { get; } = new();

    // public Guid? FlitchIdentity { get; set; }
}