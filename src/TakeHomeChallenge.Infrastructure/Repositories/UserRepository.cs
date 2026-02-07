using Microsoft.EntityFrameworkCore;
using TakeHomeChallenge.Domain.Entities;
using TakeHomeChallenge.Domain.Interfaces;

namespace TakeHomeChallenge.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<bool> CreateAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        var saved = await _dbContext.SaveChangesAsync();
        return saved > 0;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        var saved = await _dbContext.SaveChangesAsync();
        return saved > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return false;
        }

        _dbContext.Users.Remove(user);
        var saved = await _dbContext.SaveChangesAsync();
        return saved > 0;
    }

    public async Task<bool> ExistsAsync(string userName)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Name == userName);
    }
}
