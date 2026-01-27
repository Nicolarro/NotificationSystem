using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Domain.Interfaces;

public interface IUserRepository
{
    Task<ICollection<UserWithPokemonDTO>> GetAllAsync();
    Task<UserWithPokemonDTO> GetByIdAsync(int id);
    Task<UserWithPokemonDTO> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> CreateUser(User user);
    Task<bool> ExistsAsync(string userName);
}
