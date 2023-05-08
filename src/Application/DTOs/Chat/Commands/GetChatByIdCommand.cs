namespace Application.DTOs.Chat.Commands;

public class GetChatByIdCommand
{
    public GetChatByIdCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}