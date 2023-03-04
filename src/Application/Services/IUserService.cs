namespace Application.Services;

public interface IUserService
{
    public Task<string> SayHi(int userId);
}
