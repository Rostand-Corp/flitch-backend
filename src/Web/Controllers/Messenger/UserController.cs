using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Services.Users;
using Application.Services.Users.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Messenger.ViewModels;

namespace Web.Controllers.Messenger
{
    [Route("api/messenger")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [Authorize]
        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var command = new CreateUserCommand(request.DisplayName, GetFlitchIdentity());
            var response = await _userAppService.CreateUser(command);

            if (response.IsSuccess) return Ok(response.Value);

            return Problem(detail: response.Error.Message, type: response.Error.Code); // TODO: add status code
        }
        
        [Authorize]
        [Route("users/self/status")] // hmm
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(ChangeUserStatusRequest request)
        {
            var command = new ChangeStatusCommand(GetMessengerIdentity(), request.Status);
            var response = await _userAppService.ChangeUserStatus(command);

            if (response.IsSuccess) return Ok(response.Value);

            return Problem(detail: response.Error.Message, type: response.Error.Code); // TODO: add status code
        }

        private string GetFlitchIdentity()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private string GetMessengerIdentity()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "msngrUserId")?.Value;
        }
    }
}
