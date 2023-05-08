namespace Application.DTOs.Chat.Commands;

public class UpdateChatCommand
{
    public UpdateChatCommand(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
}