using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Application.Services.Users.Commands;
using Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Messenger.ViewModels;

namespace Web.Controllers.Messenger
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var command = new CreateUserCommand(request.DisplayName);
            var response = await _userAppService.CreateUser(command);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([Required] [FromRoute] string id)
        {
            var response = await _userAppService.GetUserById(id);
            
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([Range(1, int.MaxValue)] [FromQuery] int amount)
        {
            var command = new GetUsersCommand(amount);
            var response = await _userAppService.GetUsers(command);

            return Ok(response);
        }
        
        [Authorize(Policy = "MessengerId")]
        [Route("self")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
        {
            var command = new UpdateSelfCommand(request.DisplayName, request.Status);
            var response = await _userAppService.UpdateUser(command);

            return Ok(response);
        }
        
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var response = await _userAppService.DeleteUser(id);

            return Ok(response);
        }
        

        /*private string GetFlitchIdentity()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        } // Was moved to the CurrentUserService */

        /*private string GetMessengerIdentity()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "msngrUserId")?.Value;
        } // Was moved to the CurrentUserService */
    }
}
