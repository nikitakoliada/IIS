using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class EquipmentTypeRepository(ApplicationDbContext context)
{
    public Task<List<EquipmentType>> GetAllAsync()
    {
        return context.EquipmentTypes.ToListAsync();
    }
    
    public Task<EquipmentType?> GetByIdAsync(int id)
    {
        return context.EquipmentTypes.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public Task<int> CreateAsync(EquipmentType equipmentType)
    {
        context.Add(equipmentType);
        
        return context.SaveChangesAsync();
    }
    
    public Task<int> UpdateAsync(EquipmentType equipmentType)
    {
        context.Update(equipmentType);
        
        return context.SaveChangesAsync();
    }
    
    public Task<int> RemoveAsync(EquipmentType equipmentType)
    {
        context.Remove(equipmentType);
        
        return context.SaveChangesAsync();
    }
}