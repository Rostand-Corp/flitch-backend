using Domain.Entities;
using Domain.Shared;

namespace Domain.Services;

public class UserService : IUserService
{
    public Result<User> CreateUser(string displayName)
    {
        ArgumentException.ThrowIfNullOrEmpty(displayName); // TODO: Experimental
        if (displayName.Length > 16)
            return new Error("Messenger.User.Name.Validation", "Display name must be less than 16 characters long");

        return new User
        {
            Id = new Guid(), // In terms of domain independence it is the most correct option possible
            DisplayName = displayName,
            Status = "Hi! I'm new here!"
        };
    }

    public Result ChangeUserStatus(User user, string status)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrEmpty(status);
        if (status.Length > 100)
            return new Error("Messenger.User.Status.Validation", "Status must be less than 100 characters long");

        user.Status = status;

        return Result.Success();
    }
}