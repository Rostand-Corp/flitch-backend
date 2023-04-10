using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _context;
    private ClaimsPrincipal? _user;
    private Guid? _currentUserId;
    private Guid? _currentMessengerUserId;

    public CurrentUserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public Guid? UserId => _currentUserId ??= Guid.Parse(GetUser().FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                                         ?? throw new InvalidOperationException()); //todo: 

    public Guid? MessengerUserId => _currentMessengerUserId ??= Guid.Parse(GetUser().FindFirst("msngrUserId")?.Value
                                                                           ?? throw new InvalidOperationException());

    public bool IsInRole(string role) => GetUser().IsInRole(role);
    
    // public string? ConnectionId { get; private set; }

    public ClaimsPrincipal GetUser()
    {
        return _user ??= _context.HttpContext?.User!;
    }

    public void SetUser(ClaimsPrincipal user)
    {
        _user = user;
    }
}