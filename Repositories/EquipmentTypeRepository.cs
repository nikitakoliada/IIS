using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class EquipmentTypeRepository
{
    private readonly ApplicationDbContext _context;
    
    public EquipmentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task<List<EquipmentType>> GetAllAsync()
    {
        return _context.EquipmentTypes.ToListAsync();
    }
    
    public Task<EquipmentType?> GetByIdAsync(int id)
    {
        return _context.EquipmentTypes.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public Task<int> CreateAsync(EquipmentType equipmentType)
    {
        _context.Add(equipmentType);
        
        return _context.SaveChangesAsync();
    }
    
    public Task<int> UpdateAsync(EquipmentType equipmentType)
    {
        _context.Update(equipmentType);
        
        return _context.SaveChangesAsync();
    }
    
    public Task<int> RemoveAsync(EquipmentType equipmentType)
    {
        _context.Remove(equipmentType);
        
        return _context.SaveChangesAsync();
    }
}