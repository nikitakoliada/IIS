using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class StudioRepository
{
    private readonly ApplicationDbContext _context;
    
    public StudioRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task<List<Studio>> GetAllAsync()
    {
        return _context.Studios.ToListAsync();
    }
    
    public Task<Studio?> GetByIdAsync(int id)
    {
        return _context.Studios.FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public Task<int> CreateAsync(Studio studio)
    {
        _context.Add(studio);
        
        return _context.SaveChangesAsync();
    }
    
    public Task<int> UpdateAsync(Studio studio)
    {
        _context.Update(studio);
        
        return _context.SaveChangesAsync();
    }
    
    public Task<int> RemoveAsync(Studio studio)
    {
        _context.Remove(studio);
        
        return _context.SaveChangesAsync();
    }
}