using AutoMapper;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Domain -> DTO
        CreateMap<User, UserResponseDTO>()
            .ForMember(dest => dest.PokemonIds, opt => opt.MapFrom(src => src.PokemonsIds));

        // DTO -> Domain
        CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.PokemonsIds, opt => opt.MapFrom(src => src.PokemonIds))
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id is auto-generated
    }
}
