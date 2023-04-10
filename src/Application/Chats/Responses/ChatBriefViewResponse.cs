namespace Application.Chats.Responses;

public class ChatBriefViewResponse
{
    public Guid Id { get; set; }
    
    public string ChatName { get; set; }
    public MessageBriefViewResponse LastMessage { get; set; }

}