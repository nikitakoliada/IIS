using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class EquipmentRepository(ApplicationDbContext context)
{
    public Task<List<Equipment>> GetAllWithIncludesAsync()
    {
        return context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Studio)
            .ToListAsync();
    }

    public Task<Equipment?> GetByIdAsync(int id)
    {
        return context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public Task<Equipment?> GetByIdWithIncludesAsync(int id)
    {
        return context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Studio)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<int> CreateAsync(Equipment equipment)
    {
        context.Add(equipment);

        return context.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(Equipment equipment)
    {
        context.Update(equipment);

        return context.SaveChangesAsync();
    }

    public Task<int> RemoveAsync(Equipment equipment)
    {
        context.Remove(equipment);

        return context.SaveChangesAsync();
    }
}