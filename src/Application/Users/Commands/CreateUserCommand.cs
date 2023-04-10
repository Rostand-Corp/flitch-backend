
namespace Application.Services.Users.Commands;

public class CreateUserCommand
{
    public CreateUserCommand(string displayName)
    {
        DisplayName = displayName;
    }
    
    public string DisplayName { get; set; }
    // public string IdentityId { get; set; }
}