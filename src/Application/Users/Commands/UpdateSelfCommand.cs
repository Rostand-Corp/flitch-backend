namespace Application.Services.Users.Commands;

public class UpdateSelfCommand
{
    public UpdateSelfCommand(string displayName, string status)
    {
        DisplayName = displayName;
        Status = status;
    }

    // public string Id { get; set; }
    
    public string DisplayName { get; set; }
    public string Status { get; set; }
}