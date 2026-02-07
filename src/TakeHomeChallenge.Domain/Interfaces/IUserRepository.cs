using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Domain.Interfaces;

public interface IUserRepository
{
    Task<ICollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<bool> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string userName);
}
