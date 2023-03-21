namespace Application.Services.Users.ViewModels;

public record CreateUserCommand(string DisplayName, string IdentityId);
public record ChangeStatusCommand(string UserId, string Status);