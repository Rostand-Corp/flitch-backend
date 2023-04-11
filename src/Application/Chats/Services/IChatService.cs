using Application.Chats.Commands;
using Application.Chats.Responses;

namespace Application.Chats.Services;

public interface IChatService
{
    public Task<ChatFullResponse> CreatePrivateChat(CreatePrivateChatCommand command); 
    public Task<ChatFullResponse> CreateGroupChat(CreateGroupChatCommand command);
    /*public Task<ChatFullResponse> CreatePublicChat(CreatePublicChatCommand command);*/

    public Task<IEnumerable<ChatBriefViewResponse>> GetMyChats(GetMyChatsBriefViewsCommand command);
    public Task<ChatFullResponse> GetChatById(GetChatByIdCommand command);
    public Task<IEnumerable<MessageResponse>> GetChatMessages(GetChatMessagesCommand command);

    /*
    public Task<ChatFullResponse> UpdatePublicChatDetails(UpdatePublicChatCommand command);
    public Task<ChatUserBriefResponse> UpdateMemberRole(UpdateMemberRole command); // Change response maybe
    */
    public Task<MessageResponse> SendMessage(SendMessageCommand command);
    public Task<MessageResponse> UpdateMessage(UpdateMessageCommand command);
    public Task<MessageResponse> DeleteMessage(DeleteMessageCommand command);
    /*
    public Task<ChatUserBriefResponse> AddUserToGroupChat(AddUserToGroupChatCommand command);
    public Task<ChatUserBriefResponse> AddUserToPublicChat(AddUserToPublicChatCommand command);
    public Task<ChatUserBriefResponse> RemoveUserFromPublicChat(RemoveUserFromPublicChatCommand command);

    public Task<ChatFullResponse> DeletePublicChat(DeletePublicChatCommand command);*/


}