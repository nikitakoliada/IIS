using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class EquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task<List<Equipment>> GetAllWithIncludesAsync()
    {
        return _context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Studio)
            .ToListAsync();
    }

    public Task<Equipment?> GetByIdAsync(int id)
    {
        return _context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public Task<Equipment?> GetByIdWithIncludesAsync(int id)
    {
        return _context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Studio)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<int> CreateAsync(Equipment equipment)
    {
        _context.Add(equipment);

        return _context.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(Equipment equipment)
    {
        _context.Update(equipment);

        return _context.SaveChangesAsync();
    }

    public Task<int> RemoveAsync(Equipment equipment)
    {
        _context.Remove(equipment);

        return _context.SaveChangesAsync();
    }
}