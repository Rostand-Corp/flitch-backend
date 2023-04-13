using System.Security.Claims;
using Application.Chats.Commands;
using Application.Chats.Responses;
using Application.Chats.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Messenger.Chat.ViewModels;

namespace Web.Controllers.Messenger.Chat
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Creates new Chat resource (private chat)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ChatFullResponse</returns>
        /// <response code="201">Returns ChatFullResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (business / request)</response>
        /// <response code="400">Returns problem details if you are trying to create a chat with yourself</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if either creator or recipient do not exist</response>
        /// <response code="409">Returns problem details if the chat already exists</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpPost("private")]
        [ProducesResponseType(typeof(ChatFullResponse), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> CreatePrivateChat(CreatePrivateChatRequest request)
        {
            var command =
                new CreatePrivateChatCommand(Guid.Parse(request.RecipientId));

            var result = await _chatService.CreatePrivateChat(command);

            return CreatedAtAction(nameof(CreatePrivateChat), new {Id = result.Id}, result);
        }

        /// <summary>
        /// Creates new Chat resource (group chat)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ChatFullResponse</returns>
        /// <response code="201">Returns ChatFullResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (business / request)</response>
        /// <response code="400">Returns problem details if user has not specified any participants</response>
        /// <response code="400">Returns problem details if user specified only himself as only participant</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if either creator or any of the recipients(!) do not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpPost("group")]
        [ProducesResponseType(typeof(ChatFullResponse), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatRequest request)
        {
            var command = new CreateGroupChatCommand(request.UserIds);

            var result = await _chatService.CreateGroupChat(command);
            return CreatedAtAction(nameof(CreateGroupChat), new {Id = result.Id}, result);
        }
        

        /// <summary>
        /// Retrieves current user's Chats
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Array of ChatFullResponse</returns>
        /// <response code="201">Returns array of ChatFullResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (request)</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="404">Returns problem details if client does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChatFullResponse>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]   
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]        
        public async Task<IActionResult> GetMyChats([FromQuery] GetMyChatsRequest request)
        {
            var command = new GetMyChatsBriefViewsCommand(request.Amount);

            return Ok(await _chatService.GetMyChats(command));
        }

        /// <summary>                                                                                          
        /// Retrieves a Chat resource (user must be a participant)                                                                   
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="request"></param>                                                                     
        /// <returns>ChatFullResponse</returns>                                                       
        /// <response code="201">Returns ChatFullResponse</response>                                  
        /// <response code="400">Returns problem details if validation problem occurs (request)</response>     
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="403">Returns problem details if user is not in the requested chat</response>
        /// <response code="404">Returns problem details if chat does not exist</response>                   
        /// <response code="500">Returns problem details if critical internal server error occurred</response> 
        [Authorize(Policy = "MessengerId")]
        [HttpGet("{chatId}")]
        [ProducesResponseType(typeof(ChatFullResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]       
        [ProducesResponseType(typeof(ProblemDetails), 403)]  
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]   
        public async Task<IActionResult> GetChatById([FromRoute] Guid chatId, [FromQuery] GetChatByIdRequest request)
        {

            if (!Enum.TryParse(request.Flag, ignoreCase: true, out ChatDisplayFlag flag))
            {
                return BadRequest("Invalid flag value. Expected: {default}");
            }

            var command = new GetChatByIdCommand(chatId, flag);

            return Ok(await _chatService.GetChatById(command));
        }

        /// <summary>
        /// Retrieves Chat's messages
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="request"></param>
        /// <returns>Array of MessageResponse</returns>
        /// <response code="201">Returns array of MessageResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (request)</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="403">Returns problem details if user is not in the requested chat</response>
        /// <response code="404">Returns problem details if chat does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpGet("{chatId}/messages")]
        [ProducesResponseType(typeof(IEnumerable<MessageResponse>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]       
        [ProducesResponseType(typeof(ProblemDetails), 403)]  
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GetChatMessages([FromRoute] Guid chatId, [FromQuery] GetChatMessagesRequest request)
        {
            var command = new GetChatMessagesCommand(chatId, request.Amount, request.Before);

            return Ok(await _chatService.GetChatMessages(command));
        }
        
        /// <summary>
        /// Creates a Message in the Chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="request"></param>
        /// <returns>MessageResponse</returns>
        /// <response code="201">Returns a MessageResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (business / request)</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="403">Returns problem details if user is not in the requested chat</response>
        /// <response code="404">Returns problem details if chat does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpPost("{chatId}/messages")]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]       
        [ProducesResponseType(typeof(ProblemDetails), 403)]  
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> SendMessage([FromRoute] Guid chatId, [FromBody] SendMessageRequest request)
        {
            var command = new SendMessageCommand(chatId, request.Content);

            return Ok(await _chatService.SendMessage(command));
        }

        /// <summary>
        /// Updates the Message in the Chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="request"></param>
        /// <returns>MessageResponse</returns>
        /// <response code="200">Returns a MessageResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (business / request)</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="403">Returns problem details if user is not in the requested chat</response>
        /// <response code="403">Returns problem details if user is not the message author</response>
        /// <response code="404">Returns problem details if chat does not exist</response>
        /// <response code="404">Returns problem details if message does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpPut("{chatId}/messages/{messageId}")]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]       
        [ProducesResponseType(typeof(ProblemDetails), 403)]  
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> UpdateMessage([FromRoute] Guid chatId, [FromRoute] Guid messageId,
            [FromBody] UpdateMessageRequest request)
        {
            var command = new UpdateMessageCommand(chatId, messageId, request.Content);

            return Ok(await _chatService.UpdateMessage(command));
        }

        /// <summary>
        /// Deletes the Message in the Chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <returns>MessageResponse</returns>
        /// <response code="200">Returns a MessageResponse</response>
        /// <response code="400">Returns problem details if validation problem occurs (business / request)</response>
        /// <response code="401">Returns 401 if client is unauthorized</response>
        /// <response code="403">Returns problem details if user is not in the requested chat</response>
        /// <response code="403">Returns problem details if user is not the message author</response>
        /// <response code="404">Returns problem details if chat does not exist</response>
        /// <response code="404">Returns problem details if message does not exist</response>
        /// <response code="500">Returns problem details if critical internal server error occurred</response>
        [Authorize(Policy = "MessengerId")]
        [HttpDelete("{chatId}/messages/{messageId}")]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]        
        [ProducesResponseType(401)]       
        [ProducesResponseType(typeof(ProblemDetails), 403)]  
        [ProducesResponseType(typeof(ProblemDetails), 404)]        
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid chatId, [FromRoute] Guid messageId)
        {
            var command = new DeleteMessageCommand(chatId, messageId);

            return Ok(await _chatService.DeleteMessage(command));
        }
    }
}