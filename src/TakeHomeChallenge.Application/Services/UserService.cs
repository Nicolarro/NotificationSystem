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
    private readonly IPokemonService _pokemonService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPokemonService pokemonService, IMapper mapper)
    {
        _userRepository = userRepository;
        _pokemonService = pokemonService;
        _mapper = mapper;
    }

    public async Task<ICollection<UserResponseDTO>> GetUsersAsync()
    {
        var users = (await _userRepository.GetAllAsync()).ToList();
        var userDtos = _mapper.Map<List<UserResponseDTO>>(users);

        // Enrich each user with Pokemon details in parallel
        var tasks = userDtos.Select(async (dto, i) =>
        {
            var ids = users[i].PokemonsIds;
            if (ids is not null && ids.Any())
            {
                dto.PokemonDetails = (await _pokemonService.GetPokemonsByIdsAsync(ids)).ToList();
            }
        });
        await Task.WhenAll(tasks);

        return userDtos;
    }

    public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserResponseDTO>(user);

        if (user.PokemonsIds is not null && user.PokemonsIds.Any())
        {
            var details = await _pokemonService.GetPokemonsByIdsAsync(user.PokemonsIds);
            userDto.PokemonDetails = details.ToList();
        }

        return userDto;
    }

    public async Task<UserResponseDTO?> CreateUserAsync(CreateUserDTO userDto)
    {
        var user = _mapper.Map<User>(userDto);

        if (userDto.PokemonIds is not null)
        {
            user.PokemonsIds = new Collection<int>(userDto.PokemonIds.ToList());
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
            existingUser.PokemonsIds = new Collection<int>(userDto.PokemonIds);
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

