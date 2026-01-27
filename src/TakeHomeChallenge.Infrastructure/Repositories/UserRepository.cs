using TakeHomeChallenge.Domain.Interfaces;
using TakeHomeChallenge.Domain.Entities;
using TakeHomeChallenge.Application.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace TakeHomeChallenge.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _autoMapper;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<UserWithPokemonDTO>> GetAllAsync()
    {
        return await _dbContext.Users?.ToListAsync();
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        return user;
    }

    // public async Task<UserWithPokemonDTO> AddUser(User user)
    // {
    //     if (user == null)
    //     {

    //         throw new Exception(nameof(user));
    //     }
    //     var newUser = _dbContext.Users?.Add(user);
    //     await _dbContext.SaveChangesAsync();
    //     return _autoMapper.Map<UserWithPokemonDTO>(newUser?.Entity);
    // }

    public async Task<User> UpdateAsync(User user)
    {
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id) ?? throw new Exception("El usuario no existe");
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateUser(User user)
    {

        if (user == null)
        {
            throw new Exception();
        }

        _dbContext.Users?.Add(_autoMapper.Map<User>(user));

        await _dbContext.SaveChangesAsync();
        return true;
    }


    public async Task<bool> ExistsAsync(string userName)
    {
        return false;
    }
}
