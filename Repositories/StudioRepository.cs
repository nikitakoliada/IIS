using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class StudioRepository(ApplicationDbContext context)
{
    public Task<List<Studio>> GetAllAsync()
    {
        return context.Studios.ToListAsync();
    }
    
    public Task<Studio?> GetByIdAsync(int id)
    {
        return context.Studios.FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public Task<int> CreateAsync(Studio studio)
    {
        context.Add(studio);
        
        return context.SaveChangesAsync();
    }
    
    public Task<int> UpdateAsync(Studio studio)
    {
        context.Update(studio);
        
        return context.SaveChangesAsync();
    }
    
    public Task<int> RemoveAsync(Studio studio)
    {
        context.Remove(studio);
        
        return context.SaveChangesAsync();
    }
    
    public async Task<User?> GetUserWithStudioAsync(string username)
    {
        return await context.Users
            .Include(u => u.AssignedStudio)
            .FirstOrDefaultAsync(u => u.UserName == username);
    }
}