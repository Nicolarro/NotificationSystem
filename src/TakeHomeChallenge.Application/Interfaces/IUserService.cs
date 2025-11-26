using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Application.Interfaces;

public interface IUserService
{
    Task<ICollection<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> IsUniqueUserAsync(string userName);

    Task<User> CreateUser(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}
