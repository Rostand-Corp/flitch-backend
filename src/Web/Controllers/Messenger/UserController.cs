using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Application.Services.Users.Commands;
using Application.Users.Responses;
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
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var command = new CreateUserCommand(request.DisplayName);
            var response = await _userAppService.CreateUser(command);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves User resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserResponse</returns>
        /// <response code="200">Returns UserResponse</response>
        /// <response code="400">Returns problem details if query parameter is invalid</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if user does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GetUserById([Required] [FromRoute] string id)
        {
            var response = await _userAppService.GetUserById(id);
            
            return Ok(response);
        }

        /// <summary>
        /// Retrieves Users resources
        /// </summary>
        /// <param name="amount">Specifies number of records to retrieve</param>
        /// <returns>UserResponse</returns>
        /// <response code="200">Returns UserResponse</response>
        /// <response code="400">Returns problem details if query parameter is invalid</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GetUsers([Range(1, int.MaxValue)] [FromQuery] int amount)
        {
            var command = new GetUsersCommand(amount);
            var response = await _userAppService.GetUsers(command);

            return Ok(response);
        }
        
        /// <summary>
        /// Updates User resource
        /// </summary>
        /// <param name="request"></param>
        /// <returns>UserResponse</returns>
        /// <response code="200">Returns UserResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if user does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [Route("self")]
        [HttpPut]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
        {
            var command = new UpdateSelfCommand(request.DisplayName, request.Status);
            var response = await _userAppService.UpdateUser(command);

            return Ok(response);
        }
        
        /// <summary>
        /// Deletes User resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserResponse</returns>
        /// <response code="200">Returns UserResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if user does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
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
