using TakeHomeChallenge.Application.Interfaces;
using TakeHomeChallenge.Domain.Interfaces;
using TakeHomeChallenge.Domain.Entities;
using System.Runtime.InteropServices.Marshalling;

namespace TakeHomeChallenge.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ICollection<User>> GetUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<bool> IsUniqueUserAsync(string userName)
    {
        return await _userRepository.ExistsAsync(userName);
    }

    public async Task<User> CreateUser(User user)
    {
        await _userRepository.CreateUser(user);
        return user;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
        return true;
    }
}

