using System.Diagnostics.CodeAnalysis;
using Application.Services.Users.ViewModels;
using Domain.Shared;
using Domain.Services;
using Infrastructure.Auth;
using Infrastructure.Data;

namespace Application.Services.Users;

public class UserAppService : IUserAppService
{
    private readonly FlitchDbContext _db;
    private readonly IUserService _userService;
    private readonly IAuthManager _authManager;

    public UserAppService(FlitchDbContext db, IUserService userService, IAuthManager authManager)
    {
        _db = db;
        _userService = userService;
        _authManager = authManager;
    }

    public async Task<Result<UserResponse>> CreateUser([NotNull] CreateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.DisplayName); // If it is null, then programmer / system's internal fault, emptiness check is domain concern
        ArgumentException.ThrowIfNullOrEmpty(command.IdentityId); // an exception is thrown here because it programmer fault
        
        var createUserResult = _userService.CreateUser(command.DisplayName);
        if (createUserResult.IsFailure) return createUserResult.Error;

        var newUser = createUserResult.Value;

       await using var transaction = await _db.Database.BeginTransactionAsync();

       try
       {
           _db.Users.Add(newUser);

           await _db.SaveChangesAsync();

           var registerResult = await
               _authManager.RegisterInSubsystem(
                   command.IdentityId,
                   newUser.Id.ToString(),
                   Subsystems.Messenger);

           if (registerResult.IsFailure)
           {
               await transaction.RollbackAsync();
               return registerResult.Error;
           }

           await transaction.CommitAsync();
       }
       catch (Exception)
       {
           await transaction.RollbackAsync();
       }

       return new UserResponse(newUser.Id.ToString(), newUser.DisplayName, newUser.Status!);

    }

    public async Task<Result<UserResponse>> ChangeUserStatus(ChangeStatusCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Status);
        ArgumentException.ThrowIfNullOrEmpty(command.UserId);
        // var censorResult = _profanityFilter.CheckInappropriate(command.Status)
        
        var user = await _db.Users.FindAsync(Guid.Parse(command.UserId));
        ArgumentNullException.ThrowIfNull(user); // not user's concern, or is it?
        

        var changeStatusResult = _userService.ChangeUserStatus(user, command.Status);

        if (changeStatusResult.IsFailure) return changeStatusResult.Error;

        await _db.SaveChangesAsync();

        return new UserResponse(user.Id.ToString(), user.DisplayName, user.Status!);
    }
}