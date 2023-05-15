using Application.DTOs.Chat.Responses;

namespace Application.Hubs;

public interface IMessengerHub
{
    Task MessageReceived(Guid chatId, MessageResponse message);
    Task MessageUpdated(Guid chatId, MessageResponse message);
    Task MessageDeleted(Guid chatId, Guid messageId);
    Task NewChatCreated(IEnumerable<Guid> userIds, ChatFullResponse chat);
    Task ChatUpdated(ChatFullResponse chat);
    Task NewMemberAdded(Guid chatId, ChatUserBriefResponse newMember);
    Task UserLeftChat(Guid chatId, Guid userId); // TODO: Ok, do I return actual chat copy or just a notification?
}