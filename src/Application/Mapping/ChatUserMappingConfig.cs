using Application.DTOs.Chat.Responses;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public class ChatUserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ChatUser, ChatUserBriefResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ChatId, src => src.ChatId)
            .Map(dest => dest.Username, src => src.User.DisplayName)
            .Map(dest=>dest.Fullname, src=>src.User.FullName)
            .Map(dest => dest.Joined, src => src.Joined)
            .Map(dest => dest.Role, src => src.Role);

        config.NewConfig<ChatUser, ChatUserMinimalNoChatResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.User.DisplayName)
            .Map(dest => dest.Fullname, src => src.User.FullName);
    }
}