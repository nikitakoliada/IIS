using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class BorrowRepository(ApplicationDbContext context)
{
    public Task<List<Borrow>> GetAllWithIncludesAsync()
    {
        return context.Reservations
            .Include(e => e.User)
            .Include(e => e.Equipment)
            .ToListAsync();
    }
    
    public Task<List<Borrow>> GetByUserId(string userId)
    {
        return context.Reservations
            .Where(x => x.UserId == userId)
            .Include(e => e.User)
            .Include(e => e.Equipment)
            .ToListAsync();
    }
    
    public Task<List<Borrow>> GetByEquipmentId(int equipmentId)
    {
        return context.Reservations
            .Where(x => x.EquipmentId == equipmentId)
            .Include(e => e.User)
            .Include(e => e.Equipment)
            .ToListAsync();
    }
    
    public Task<List<Borrow>> GetByStudioId(int studioId)
    {
        return context.Reservations
            .Include(e => e.User)
            .Where(x => x.User.AssignedStudioId == studioId)
            .Include(e => e.Equipment)
            .ToListAsync();
    }

    public Task<Borrow?> GetByIdAsync(int id)
    {
        return context.Reservations
            .Include(e => e.User)
            .Include(e => e.Equipment)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<int> CreateAsync(Borrow borrow)
    {
        context.Add(borrow);

        return context.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(Borrow borrow)
    {
        context.Update(borrow);

        return context.SaveChangesAsync();
    }

    public Task<int> RemoveAsync(Borrow borrow)
    {
        context.Remove(borrow);

        return context.SaveChangesAsync();
    }
}