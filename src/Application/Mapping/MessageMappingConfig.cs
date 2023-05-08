using Application.DTOs.Chat.Responses;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public class MessageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Message, MessageResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ChatId, src => src.ChatId)
            .Map(dest => dest.Sender, src => src.Sender)
            .Map(dest => dest.InReplyTo, src => src.ReplyTo)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.Timestamp, src => src.Timestamp);

        config.NewConfig<Message, MessageReplyResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Sender.User.DisplayName)
            .Map(dest=>dest.Fullname, src=>src.Sender.User.FullName)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.Timestamp, src => src.Timestamp);

        config.NewConfig<Message, MessageBriefViewResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.AuthorUserName, src => src.Sender.User.DisplayName)
            .Map(dest=>dest.AuthorFullName, src=>src.Sender.User.FullName)
            .Map(dest => dest.Timestamp, src => src.Timestamp);
    }
}