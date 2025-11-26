using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Domain.Interfaces;

public interface IUserRepository
{
    Task<ICollection<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task<User> AddUser(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<User> CreateUser(User user);
    Task<bool> ExistsAsync(string userName);
}
