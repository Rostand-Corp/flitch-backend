using Domain.Entities;

namespace Application.DTOs.Chat.Responses;

public class ChatBriefViewResponse
{
    public Guid Id { get; set; }
    
    public string ChatName { get; set; }
    public ChatType Type { get; set; }
    public IEnumerable<ChatUserBriefResponse> Participants { get; set; } 
    public MessageBriefViewResponse LastMessage { get; set; }

}