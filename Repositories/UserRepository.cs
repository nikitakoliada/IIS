using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class UserRepository(ApplicationDbContext context)
{
    public Task<List<User>> GetAllAsync()
    {
        return context.Users.ToListAsync();
    }
    
    public Task<List<User>> GetAllWithIncludesAsync()
    {
        return context.Users
            .Include(u => u.AssignedStudio)
            .ToListAsync();
    }
    
    public Task<List<User>> GetAllFromAndWithoutStudioWithIncludesFromStudioAsync(int studioId)
    {
        return context.Users
            .Include(u => u.AssignedStudio)
            .Where(u => u.AssignedStudioId == studioId || u.AssignedStudioId == null)
            .ToListAsync();
    }

    public Task<List<User>> GetAllFromStudioWithIncludesFromStudioAsync(int studioId)
    {
        return context.Users
            .Include(u => u.AssignedStudio)
            .Where(u => u.AssignedStudioId == studioId)
            .ToListAsync();
    }

    public Task<User?> GetByIdAsync(string id)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public Task<bool> ExistsAsync(string id)
    {
        return context.Users.AnyAsync(u => u.Id == id);
    }

    public Task<int> CreateAsync(User user)
    {
        context.Add(user);

        return context.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(User user)
    {
        context.Update(user);

        return context.SaveChangesAsync();
    }

    public Task<int> RemoveAsync(User user)
    {
        context.Remove(user);

        return context.SaveChangesAsync();
    }
}