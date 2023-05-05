using Application.Chats.Responses;
using Domain.Entities;
using Mapster;

namespace Application.Chats.Mappings;

public class ChatMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Chat, ChatFullResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Participants, src => src.Participants)
            .Map(dest => dest.Messages, src => src.Messages);

        config.NewConfig<Chat, ChatBriefViewResponse>()
            .Map(dest => dest.Id, src => src.Id)    
            .Map(dest => dest.ChatName, src => src.Name)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Participants, src => src.Participants)
            .Map(dest => dest.LastMessage, src => src.LastMessage);
    }
}