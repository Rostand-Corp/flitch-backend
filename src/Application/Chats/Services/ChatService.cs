using System.Net;
using Application.Chats.Commands;
using Application.Chats.Responses;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.User;
using FluentValidation;
using Infrastructure.Data;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Application.Chats.Services;

public class ChatService : IChatService
{
    private readonly FlitchDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<Message> _messageValidator;

    public ChatService(FlitchDbContext db, IMapper mapper, ICurrentUserService currentUserService, IValidator<Message> messageValidator)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUserService;
        _messageValidator = messageValidator;
    }
    
    public async Task<ChatFullResponse> CreatePrivateChat(CreatePrivateChatCommand command) // need complete error handling support
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (command.UserId == Guid.Empty) // arguable
        {
            throw new ArgumentException("User ID must not be empty.");
        }

        if (command.UserId == _currentUser.MessengerUserId)
        {
            throw new ValidationException("Chat", new Dictionary<string, string[]>() // is this even validation exception?
            {
                ["Participants"] = new []{"You cannot create a group chat with yourself."}
            });
        }
        
        var user1 = await _db.Users.FindAsync(_currentUser.MessengerUserId) ??
                    throw new NotFoundException("Chat.User.NotFound", "The creator user was not found.");
        var user2 = await _db.Users.FindAsync(command.UserId) ??
                    throw new NotFoundException("Chat.User.NotFound", "The invited user was not found.");


        var chatAlreadyExists = 
            await _db.Chats.AnyAsync(c => 
                c.Type == ChatType.Private &&
                c.Users.Contains(user1) &&
                c.Users.Contains(user2));

        if (chatAlreadyExists) throw new AlreadyExistsException("Chat", "This chat already exists.");

        var chat = new Chat
        {
            Id = Guid.NewGuid(), // ... .. . . ..  check this
            Type = ChatType.Private,
            Participants = new List<ChatUser>
            {
                new ChatUser
                {
                    User = user1,
                    Role = ChatRole.Participant
                },
                new ChatUser
                {
                    User = user2,
                    Role = ChatRole.Participant
                }
            }
        };
        
        var added =  _db.Chats.Add(chat);
        
        await _db.SaveChangesAsync();

        return _mapper.Map<ChatFullResponse>(chat);
    }

    public async Task<ChatFullResponse> CreateGroupChat(CreateGroupChatCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!command.UserIds.Any())
        {
            throw new ValidationException("Chat", new Dictionary<string, string[]>()
            {
                ["Participants"] = new []{"You must specify at least 1 user to create group chat with."}
            });
        }

        if (command.UserIds.Any(id => id == null))
        {
            throw new Exception("null in ids");
        }

        var creatorUser = await _db.Users.FindAsync(_currentUser.MessengerUserId) ??
                          throw new NotFoundException("Chat.User.NotFound", "The creator user was not found");
        
        var users = await _db.Users.Where(u => command.UserIds.Contains(u.Id)).ToListAsync();

        var missingUsersGuids = command.UserIds.Except(users.Select(u => u.Id)).ToList();
        if (missingUsersGuids.Count > 1)
        {
            throw new NotFoundException("Chat.Participants",
                $"The following Guids were not found: {string.Join(", ", missingUsersGuids)}");
        }

        if (users.Count == 1)
        {
            if (users.Single().Id == _currentUser.MessengerUserId)
            {
                throw new ValidationException("Chat", new Dictionary<string, string[]>() // is this even validation exception?
                {
                    ["Participants"] = new []{"You cannot create a group chat with yourself."}
                });
            }
        }

        users.Add(creatorUser);
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Group,
            Participants = users.Select(u => new ChatUser
            {
                User = u,
                Role = ChatRole.Participant
            }).ToList()
        };
        
        _db.Chats.Add(chat);

        await _db.SaveChangesAsync();

        return _mapper.Map<ChatFullResponse>(chat);
    }

    public async Task<IEnumerable<ChatBriefViewResponse>> GetMyChats(GetMyChatsBriefViewsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Amount <= 0)
            throw new ValidationException("User.Chats", new Dictionary<string, string[]>()
            {
                ["Pagination"] = new []{"You must retrieve one or more records."}
            });
        if (command.PageNumber <= 0)
            throw new ValidationException("User.Chats", new Dictionary<string, string[]>()
            {
                ["Pagination"] = new[] {"You must specify a page with an index bigger than 0."}
            });

        var user = await _db.Users.FindAsync(_currentUser.MessengerUserId)
                   ?? throw new UserNotFoundException();

        var query = _db.Entry(user)
            .Collection(u => u.Chats)
            .Query()
            .Include(c => c.LastMessage)
            .ThenInclude(m => m.Sender)
            .ThenInclude(cu => cu.User)
            .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.Timestamp : c.Created)
            .AsNoTracking();

        if (command.Filter != ChatTypeFilter.All)
        {
            if (command.Filter == ChatTypeFilter.Private)
            {
                query = query.Where(c => c.Type == ChatType.Private);
            }
        }

        var requestedChats = await query
                .Skip((command.PageNumber - 1) * command.Amount)
                .Take(command.Amount)
                .ProjectToType<ChatBriefViewResponse>()
                .ToListAsync();

        return requestedChats;
    }

    public async Task<ChatFullResponse> GetChatById(GetChatByIdCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var chat = await _db.Chats
                       .AsNoTracking()
                       .Include(c=>c.Participants)
                        .ThenInclude(cu=>cu.User)
                       .Include(c=>c.Messages.Where(m=>m.IsVisible))
                        .ThenInclude(m=>m.Sender)
                            .ThenInclude(cu=>cu.User)
                       .SingleOrDefaultAsync(c=>c.Id == command.Id)
                   ?? throw new NotFoundException("Chat.NotFound", "The specified chat does not exist.");

        if (!chat.Participants.Any(u => u.UserId == _currentUser.MessengerUserId))
            throw new RestrictedException("Chat.NotParticipant",
                "You are not a participant of this chat.");

        var creator = chat.Participants.FirstOrDefault(cu => cu.Role == ChatRole.Creator);
        var response = _mapper.Map<ChatFullResponse>(chat);
        response.CreatorId = creator?.Id;
        response.CreatorName = creator?.User.DisplayName; // To some dto maybe
        
        return _mapper.Map<ChatFullResponse>(chat);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <remarks>If both PageNumber and Before parameters are set, only Before parameter is applied</remarks>
    public async Task<IEnumerable<MessageResponse>> GetChatMessages(GetChatMessagesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Id);

       if (command.Amount <= 0)
            throw new ValidationException("Chat.Messages", new Dictionary<string, string[]>()
            {
                ["Pagination"] = new []{"You must retrieve one or more records."}
            });
        if (command.PageNumber <= 0)
            throw new ValidationException("Chat.Messages", new Dictionary<string, string[]>()
            {
                ["Pagination"] = new[] {"You must specify a page with an index bigger than 0."}
            });

        var chat = await _db.Chats
                       .AsNoTracking()
                       .Include(c=>c.Participants)
                       .Include(c=>c.Messages.Where(m=>m.IsVisible))
                       .ThenInclude(m=>m.Sender)
                       .ThenInclude(cu=>cu.User)
                       .SingleOrDefaultAsync(c=>c.Id == command.Id)
                   ?? throw new NotFoundException("Chat.NotFound", "The specified chat does not exist.");
        
        if (!chat.Participants.Any(u => u.UserId == _currentUser.MessengerUserId))
            throw new RestrictedException("Chat.NotParticipant",
                "You are not a participant of this chat.");

        var messages = chat.Messages.OrderByDescending(m => m.Timestamp); // no multiple enumeration!! but it is not a problem here anyway
        
        if (command.Before is not null)
        {
            var specificMessage = messages.SingleOrDefault(m => m.Id == command.Before) ??
                                  throw new NotFoundException("Chat.Message.NotFound",
                                      "The specified message does not exist in this chat");
            var messagesBeforeSpecific = messages
                .Where(m => m.Timestamp < specificMessage.Timestamp)
                .OrderByDescending(m => m.Timestamp)
                .Take(command.Amount);

            return _mapper.Map<IEnumerable<MessageResponse>>(messagesBeforeSpecific);
        }
        
        if (!string.IsNullOrWhiteSpace(command.SearchWord))
        {
            messages = messages
                .Where(c => c.Content
                    .Contains(command.SearchWord, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m=>m.Timestamp);
        }

        var messagesForPage = messages
            .Skip((command.PageNumber - 1) * command.Amount)
            .Take(command.Amount);

        return _mapper.Map<IEnumerable<MessageResponse>>(messagesForPage);
    }

    public async Task<MessageResponse> SendMessage(SendMessageCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.ChatId);
        ArgumentException.ThrowIfNullOrEmpty(command.Content);

        if (command.Content.Length > 500)
            throw new ValidationException("Chat.Message",
                new Dictionary<string, string[]>()
                {
                    ["Message"] = new []{"Message must be less than 500 characters long."}
                });
    
        await using var transaction = await _db.Database.BeginTransactionAsync(); // I dont think I have to wrap ENTIRE process
        try
        {

            var chat = await _db.Chats.FindAsync(command.ChatId) ??
                       throw new NotFoundException("Chat.NotFound", "The specified chat does not exist.");

            await _db.Chats.Entry(chat).Collection(c => c.Participants).LoadAsync();

            var participant = chat.Participants.FirstOrDefault(u => u.UserId == _currentUser.MessengerUserId) ??
                              throw new RestrictedException("Chat.NotParticipant",
                                  "You are not a participant of this chat.");

            var message = new Message
            {
                Sender = participant,
                Content = command.Content,
                Type = MessageType.Default,
                Timestamp = DateTime.UtcNow
            };

            chat.Messages.Add(message);

            await _db.SaveChangesAsync();

            await _db.Entry(chat).ReloadAsync();
            
            chat.LastMessage = message;

            await _db.SaveChangesAsync();
            
            await _db.Entry(message)
                .Reference(m => m.Sender)
                .LoadAsync(); // needed just to include user name to response. idk if it is needed enough
            await _db.Entry(message.Sender)
                .Reference(cu=>cu.User)
                .LoadAsync();

            await transaction.CommitAsync();

            return _mapper.Map<MessageResponse>(message);
        }
        
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<MessageResponse> UpdateMessage(UpdateMessageCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.ChatId);
        ArgumentNullException.ThrowIfNull(command.MessageId);
        
        if (command.Content.Length > 500)
            throw new ValidationException("Chat",
                new Dictionary<string, string[]>()
                {
                    ["Message"] = new []{"Message must be less than 500 characters long."}
                });
        
        var chat = await _db.Chats.FindAsync(command.ChatId) ??
                   throw new NotFoundException("Chat.NotFound", "The specified chat does not exist");
        
        await _db.Entry(chat).Collection(c => c.Messages).LoadAsync();
        var updatedMessage = chat.Messages.SingleOrDefault(m => m.Id == command.MessageId) ??
                             throw new NotFoundException("Chat.Message.NotFound",
                                 "The specified message does not exist in this chat");
        
        if (!updatedMessage.IsVisible)
            throw new NotFoundException("Chat.Message.NotFound",
                "The specified message does not exist in this chat (deleted for debug)");
        
        await _db.Entry(updatedMessage).Reference(m => m.Sender).LoadAsync();
        await _db.Entry(updatedMessage.Sender).Reference(u => u.User).LoadAsync();
        
        var isMessageOwner = updatedMessage.Sender.UserId == _currentUser.MessengerUserId;
        if (!isMessageOwner)
            throw new RestrictedException("Chat.Message.NotOwner",
                "You are not the owner of this message.");

        updatedMessage.Content = command.Content;

        _messageValidator.ValidateAndThrow(updatedMessage);
        
        _db.Update(updatedMessage);

        await _db.SaveChangesAsync();

        return _mapper.Map<MessageResponse>(updatedMessage);
    }

    public async Task<MessageResponse> DeleteMessage(DeleteMessageCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.ChatId);
        ArgumentNullException.ThrowIfNull(command.MessageId);

        var chat = await _db.Chats.FindAsync(command.ChatId) ??
                   throw new NotFoundException("Chat.NotFound", "The specified chat does not exist");
        
        await _db.Entry(chat).Collection(c => c.Messages).LoadAsync();
        var deletedMessage = chat.Messages.SingleOrDefault(m => m.Id == command.MessageId) ??
                             throw new NotFoundException("Chat.Message.NotFound",
                                 "The specified message does not exist in this chat");

        if (!deletedMessage.IsVisible)
            throw new NotFoundException("Chat.Message.NotFound",
                "The specified message does not exist in this chat (deleted for debug)");
        
        await _db.Entry(deletedMessage).Reference(m => m.Sender).LoadAsync();
        await _db.Entry(deletedMessage.Sender).Reference(u => u.User).LoadAsync();

        var isAdmin = await _db.Entry(chat)
            .Collection(c => c.Participants)
            .Query()
            .AnyAsync(u =>
            u.UserId == _currentUser.MessengerUserId && u.Role == ChatRole.Creator);

        if (!isAdmin)
        {
            var isMessageOwner = deletedMessage.Sender.UserId == _currentUser.MessengerUserId;
            if (!isMessageOwner)
                throw new RestrictedException("Chat.Message.NotOwner", 
                    "You are not the owner of this message.");
        }

        deletedMessage.IsVisible = false;
        
        if (chat.LastMessageId == deletedMessage.Id)
        {
            var newLastMessage = chat.Messages.OrderByDescending(m => m.Timestamp)
                .FirstOrDefault(m => m.IsVisible);

            chat.LastMessage = newLastMessage;
        }

        _db.Update(deletedMessage); // maybe making things a little bit more explicit, but is not needed at all

        await _db.SaveChangesAsync();

        return _mapper.Map<MessageResponse>(deletedMessage);
    }

    public async Task<ChatUserBriefResponse> LeaveGroup(LeaveGroupCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.ChatId);
        
        var chat = await _db.Chats
                       .Include(c=>c.Participants)
                       .SingleOrDefaultAsync(c=> c.Id == command.ChatId) ??
                   throw new NotFoundException("Chat.NotFound", "The specified chat does not exist");
        
        var currentUser = chat.Participants.FirstOrDefault(u => u.UserId == _currentUser.MessengerUserId && u.IsActive) ??
                          throw new RestrictedException("Chat.NotParticipant",
                              "You are not a participant of this chat.");

        currentUser.IsActive = false;

        await _db.SaveChangesAsync();
        
        return _mapper.Map<ChatUserBriefResponse>(currentUser);


    }
}