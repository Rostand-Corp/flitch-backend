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
    private readonly ILogger<MessengerHub> _logger;

    private readonly IUserConnectionMap<Guid> _connections;
    private readonly HashSet<(Guid, string)> _userConnections = new();

    public MessengerHub(ICurrentUserService currentUser, ILogger<MessengerHub> logger, IUserConnectionMap<Guid> connections)
    {
        _currentUser = currentUser;
        _logger = logger;
        _connections = connections;
    }

    public Task JoinChatHub(Guid chatId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public Task LeaveChatHub(Guid chatId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public bool CheckIsUserOnline(Guid userId)
    {
        return _connections.GetConnections(userId).Any();
    }
    
    public override Task OnConnectedAsync()
    {
        _currentUser.SetConnectionId(Context.ConnectionId);
        if (_currentUser.MessengerUserId is not null)
        {
            _connections.Add(_currentUser.MessengerUserId.Value, Context.ConnectionId);
        }

        var userName = _currentUser.GetUser().Identity?.Name;
        _logger.LogInformation("+ {UserName} has connected with connectionId {ConnectionId}", userName, Context.ConnectionId);
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (_currentUser.MessengerUserId != null)
            _connections.Remove(_currentUser.MessengerUserId.Value, Context.ConnectionId);
        
        var userName = _currentUser.GetUser().Identity?.Name;
        _logger.LogInformation("- {UserName} has disconnected with connectionId {ConnectionId}", userName, Context.ConnectionId);
        
        return base.OnDisconnectedAsync(exception);
    }
}   