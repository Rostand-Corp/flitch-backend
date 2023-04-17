namespace Application.Services.Users.Commands;

public class UpdateSelfCommand
{
    public UpdateSelfCommand(string displayName, string fullName, string status)
    {
        DisplayName = displayName;
        FullName = fullName;
        Status = status;
    }

    // public string Id { get; set; }
    
    public string DisplayName { get; set; }
    public string FullName { get; set; }
    public string Status { get; set; }
}