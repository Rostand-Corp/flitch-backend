using Domain.Entities;
using Domain.Shared;

namespace Domain.Services;

public interface IUserService
{
    public Result<User> CreateUser(string displayName);
    public Result ChangeUserStatus(User user, string status);
}