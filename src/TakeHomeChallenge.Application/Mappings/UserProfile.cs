using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Application.Mappings;

public class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDTO, User >().ReverseMap();
        CreateMap<UserResponseDTO,User>().ReverseMap();
        CreateMap<UserWithPokemonDTO, User>().ReverseMap();
    }
    
}
