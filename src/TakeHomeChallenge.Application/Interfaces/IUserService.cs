using TakeHomeChallenge.Application.DTOs;

namespace TakeHomeChallenge.Application.Interfaces;

public interface IUserService
{
    Task<ICollection<UserResponseDTO>> GetUsersAsync();
    Task<UserResponseDTO?> GetUserByIdAsync(int id);
    Task<UserResponseDTO?> CreateUserAsync(CreateUserDTO userDto);
    Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> IsUniqueUserAsync(string userName);
}
