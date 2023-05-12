using Domain.Entities;

namespace Application.DTOs.Chat.Responses;

public class ChatUserBriefResponse
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public string Username { get; set; }
    public string Fullname { get; set; }
    public ChatRole Role { get; set; }
    public DateTime Joined { get; set; }
    
}