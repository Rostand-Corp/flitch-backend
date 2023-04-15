using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Application.Services.Users.Commands;
using Application.Users.Responses;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.User;
using Domain.Validators;
using Infrastructure.Auth;
using Infrastructure.Data;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Services;

public class UserAppService : IUserAppService
{
    private readonly FlitchDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UserAppService(FlitchDbContext db, IMapper mapper, ICurrentUserService currentUserService)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUserService;
    }
    
    // The user is created initially in the AuthManager. At this point there is no need to separate this logic.
    
    public async Task<UserResponse> GetUserById(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        if (!Guid.TryParse(id, out var userId)) throw new InvalidIdentifierException();

        var user = await _db.Users.FindAsync(userId);
        
        if (user is null) throw new UserNotFoundException();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetUsers(GetUsersCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Amount <= 0) throw new FlitchException("User.Pagination", "You must retrieve one or more records");

        var users = 
            await _db.Users
                .AsNoTracking()
                .OrderByDescending(u => u.Id)
                .Take(command.Amount)
                .ProjectToType<UserResponse>()
                .ToListAsync();

        return users;
    }

    public async Task<UserResponse> UpdateUser(UpdateSelfCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        ArgumentNullException.ThrowIfNull(command.DisplayName);
        ArgumentNullException.ThrowIfNull(command.Status);

        var user = await _db.Users.FindAsync(_currentUser.MessengerUserId);
        if (user is null) throw new UserNotFoundException();

        user.DisplayName = command.DisplayName;
        user.Status = command.Status;

        var validationResult = new UserValidator().Validate(user);
        if (!validationResult.IsValid) throw new ValidationException("User", validationResult.ToDictionary());
        
        await _db.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> DeleteUser(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        
        if (!Guid.TryParse(id, out var userId)) throw new InvalidIdentifierException();
        
        var user = await _db.Users.FindAsync(userId);
        
        if (user is null) throw new UserNotFoundException();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        
        return _mapper.Map<UserResponse>(user);
    }
}