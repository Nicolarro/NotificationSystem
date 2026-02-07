using AutoMapper;
using System.Collections.ObjectModel;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Application.Interfaces;
using TakeHomeChallenge.Domain.Entities;
using TakeHomeChallenge.Domain.Interfaces;

namespace TakeHomeChallenge.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<UserResponseDTO>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<ICollection<UserResponseDTO>>(users);
    }

    public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task<UserResponseDTO?> CreateUserAsync(CreateUserDTO userDto)
    {
        var user = _mapper.Map<User>(userDto);

        if (userDto.PokemonIds is not null)
        {
            user.PokemonsIds = new List<int>(userDto.PokemonIds);
        }

        var created = await _userRepository.CreateAsync(user);
        if (!created)
        {
            return null;
        }

        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null)
        {
            return false;
        }

        // Apply only the fields that were provided (partial update)
        if (userDto.Name is not null)
        {
            existingUser.Name = userDto.Name;
        }

        if (userDto.Email is not null)
        {
            existingUser.Email = userDto.Email;
        }

        if (userDto.Password is not null)
        {
            existingUser.Password = userDto.Password;
        }

        if (userDto.PokemonIds is not null)
        {
            existingUser.PokemonsIds = new List<int>(userDto.PokemonIds);
        }

        return await _userRepository.UpdateAsync(existingUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> IsUniqueUserAsync(string userName)
    {
        return !await _userRepository.ExistsAsync(userName);
    }
}

