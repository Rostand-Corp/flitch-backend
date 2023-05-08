using System.Security.Claims;

namespace Application.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? MessengerUserId { get; }

    // string? ConnectionId { get; }

    bool IsInRole(string role);
    
    public ClaimsPrincipal GetUser();
    
    public void SetUser(ClaimsPrincipal user);
    // public void SetConnectionId();
}