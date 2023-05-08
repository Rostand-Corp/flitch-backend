using Application.DTOs.User.Commands;
using Application.DTOs.User.Responses;

namespace Application.AppServices.User;

public interface IUserAppService
{
    public Task<UserResponse> GetUserById(string id);
    public Task<IEnumerable<UserResponse>> GetUsers(GetUsersCommand command);
    public Task<UserResponse> UpdateUser(UpdateSelfCommand command);
    public Task<UserResponse> DeleteUser(string id);

}