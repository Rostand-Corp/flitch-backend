using System.ComponentModel.DataAnnotations;
using Application.Services;
using Application.Services.Users.Commands;
using Application.Users.Responses;
using Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.User;

namespace Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserAppService _userAppService;
        private ICurrentUserService _currentUser;

        public UserController(IUserAppService userAppService, ICurrentUserService currentUser)
        {
            _userAppService = userAppService;
            _currentUser = currentUser;
        }
        

        /// <summary>
        /// Retrieves CURRENT User resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserResponse</returns>
        /// <response code="200">Returns UserResponse</response>
        /// <response code="400">Returns problem details if query parameter is invalid</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if user does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpGet("self")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GetSelf()
        {
            var response = await _userAppService.GetUserById(_currentUser.MessengerUserId.ToString()!); // TODO: standardize string / guid
            
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
        /// <param name="request">Specifies request parameters</param>
        /// <returns>Array of UserResponse</returns>
        /// <response code="200">Returns array of UserResponse</response>
        /// <response code="400">Returns validation problem details if query parameters are invalid</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
        {
            var command = new GetUsersCommand(request.PageNumber ?? 1, request.Amount ?? 50, request.SearchKeyWord!);
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
            var command = new UpdateSelfCommand(request.DisplayName, request.FullName, request.Status);
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
