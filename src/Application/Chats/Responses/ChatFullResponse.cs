using Domain.Entities;

namespace Application.Chats.Responses;

public class ChatFullResponse
{
    public Guid Id { get; set; }
    
    public Guid? CreatorId { get; set; } // To dto?
    public string? CreatorName { get; set; }
    
    public string? Name { get; set; }
    
    public ChatType Type { get; set; }
    
    public IEnumerable<ChatUserBriefResponse> Participants { get; set; } 
    public IEnumerable<MessageResponse> Messages { get; set; }
}