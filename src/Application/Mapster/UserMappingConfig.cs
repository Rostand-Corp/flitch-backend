using Application.DTOs.User.Responses;
using Domain.Entities;
using Mapster;

namespace Application.Mapster;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.DisplayName, src => src.DisplayName)
            .Map(dest=>dest.FullName, src=>src.FullName)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ColorHexCode, src => src.Color == null  ? "#FFFFFF" : src.Color.HexCode);
    }
}