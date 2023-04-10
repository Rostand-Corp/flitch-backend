namespace Application.Chats.Commands;

public class GetChatByIdCommand
{
    public GetChatByIdCommand(Guid id, ChatDisplayFlag flag = ChatDisplayFlag.Default)
    {
        Id = id;
        Flag = flag;
    }
    
    public Guid Id { get; set; }
    public ChatDisplayFlag Flag { get; set; }
}

public enum ChatDisplayFlag
{
    Default
}