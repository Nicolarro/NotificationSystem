using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Application.Interfaces;

public interface IUserService
{
    Task<ICollection<UserWithPokemonDTO>> GetUsersAsync();
    Task<UserWithPokemonDTO?> GetUserByIdAsync(int id);
    Task<bool> IsUniqueUserAsync(string userName);

    Task<UserWithPokemonDTO> CreateUser(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}
