using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class UserService : IUserService
{
    public async Task<string> SayHi(int userId)
    {
        return await Task.FromResult($"Hi-Hi from {userId}!");
    }
}
