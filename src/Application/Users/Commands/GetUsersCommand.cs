namespace Application.Services.Users.Commands;

public class GetUsersCommand
{
    public GetUsersCommand(int amount) => Amount = amount;
    public int Amount { get; set; }
}