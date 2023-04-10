namespace Domain.Entities;

public class ChatUser
{
    public Guid Id { get; set; } // could be composite but nvm

    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public List<Message> Messages { get;} = new();

    public ChatRole Role { get; set; }
    public DateTime Joined { get; set; }
}

public enum ChatRole
{
    Participant,
    Creator
}