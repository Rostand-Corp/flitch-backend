using Application.Services.Users.Commands;
using Application.Users.Responses;

namespace Application.Users.Services;

public interface IUserAppService
{
    public Task<UserResponse> GetUserById(string id);
    public Task<IEnumerable<UserResponse>> GetUsers(GetUsersCommand command);
    public Task<UserResponse> UpdateUser(UpdateSelfCommand command);
    public Task<UserResponse> DeleteUser(string id);

}