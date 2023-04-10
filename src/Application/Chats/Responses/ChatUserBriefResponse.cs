using Domain.Entities;

namespace Application.Chats.Responses;

public class ChatUserBriefResponse
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public string Username { get; set; }
    public ChatRole Role { get; set; }
    public DateTime Joined { get; set; }
    
}