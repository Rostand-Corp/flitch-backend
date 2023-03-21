using Application.Services.Users.ViewModels;
using Domain.Shared;

namespace Application.Services.Users;

public interface IUserAppService
{
    public Task<Result<UserResponse>> CreateUser(CreateUserCommand command);
    public Task<Result<UserResponse>> ChangeUserStatus(ChangeStatusCommand command);

}