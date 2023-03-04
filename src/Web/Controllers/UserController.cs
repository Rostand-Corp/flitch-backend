using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet(Name = "SayHi!")]  
    public async Task<IActionResult> Hi(int id)
    {
        _logger.LogInformation($"Saying hi with {id}...");
        return Ok(await _userService.SayHi(id));
    }
}