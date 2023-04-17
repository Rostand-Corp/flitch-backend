namespace Application.Chats.Responses;

public class MessageBriefViewResponse
{
    public Guid Id { get; set; }
    
    public string AuthorUserName { get; set; }
    public string AuthorFullName { get; set; }

    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}