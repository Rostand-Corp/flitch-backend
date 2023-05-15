using System.Collections.Immutable;
using Application.DTOs.Chat.Responses;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace Application.Hubs;

[Authorize]
public class MessengerHub : Hub<IMessengerHub>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger _logger;
    
    private readonly HashSet<(Guid, string)> _userConnections = new();

    public MessengerHub(ICurrentUserService currentUser, ILogger logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public Task JoinChatHub(Guid chatId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public Task LeaveChatHub(Guid chatId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task SendMessageInChat(Guid chatId, MessageResponse message)
    {
        await Clients.Group(chatId.ToString()).MessageReceived(chatId, message);
    }
    
    public async Task UpdateMessageInChat(Guid chatId, MessageResponse message)
    {
        await Clients.Group(chatId.ToString()).MessageUpdated(chatId, message);
    }
    
    public async Task DeleteMessageInChat(Guid chatId, Guid messageId)
    {
        await Clients.Group(chatId.ToString()).MessageDeleted(chatId, messageId);
    }
    
    public async Task CreateNewChat(IEnumerable<Guid> userIds, ChatFullResponse chat)
    {
        var ids = userIds
            .Where(userId => _userConnections.Any(connection => connection.Item1 == userId))
            .ToImmutableList();
        
        await Clients.Clients(ids.Select(u=>u.ToString())).NewChatCreated(ids, chat);
    }

    public async Task UpdateChatInfo(Guid chatId, ChatFullResponse chat)
    {
        await Clients.Group(chatId.ToString()).ChatUpdated(chat);
    }

    public async Task AddNewMemberToChat(Guid chatId, Guid newMemberMessengerId, ChatUserBriefResponse newMember)
    {
        var newMemberConnection = _userConnections.FirstOrDefault(connection => connection.Item1 == newMemberMessengerId);

        if (newMemberConnection != default)
        {
            await Groups.AddToGroupAsync(newMemberConnection.Item2, chatId.ToString());
        }
        
        await Clients.Group(chatId.ToString()).NewMemberAdded(chatId, newMember);
    }
    
    public async Task LeaveChatForUser(Guid chatId, Guid userId)
    {
        var chatIdString = chatId.ToString(); // An optimization.

        await Clients.Group(chatIdString).UserLeftChat(chatId, userId); // TODO: Ask frontend if they like this approach. Looks kind of too complicated.
        await LeaveChatHub(chatId);
    }

    public override Task OnConnectedAsync()
    {
        _currentUser.SetConnectionId(Context.ConnectionId);
        if (_currentUser.MessengerUserId is not null)
        {
            _userConnections.Add((_currentUser.MessengerUserId.Value, Context.ConnectionId));
        }
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (_currentUser.MessengerUserId != null)
            _userConnections.Remove((_currentUser.MessengerUserId.Value, Context.ConnectionId));
        
        return base.OnDisconnectedAsync(exception);
    }
}   