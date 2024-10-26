using IIS.Data;
using IIS.Models;
using Microsoft.EntityFrameworkCore;

namespace IIS.Repositories;

public class RentalDayIntervalRepository 
{
    private readonly ApplicationDbContext _context;

    public RentalDayIntervalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RentalDayInterval rentalDayInterval)
    {
        _context.RentalDayIntervals.Add(rentalDayInterval);
        await _context.SaveChangesAsync();
    }

    public async Task<RentalDayInterval?> GetByIdAsync(int id)
    {
        return await _context.RentalDayIntervals.FindAsync(id);
    }

    public async Task<IEnumerable<RentalDayInterval>> GetAllAsync()
    {
        return await _context.RentalDayIntervals.ToListAsync();
    }

    public async Task UpdateAsync(RentalDayInterval rentalDayInterval)
    {
        _context.RentalDayIntervals.Update(rentalDayInterval);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rentalDayInterval = await _context.RentalDayIntervals.FindAsync(id);
        if (rentalDayInterval != null)
        {
            _context.RentalDayIntervals.Remove(rentalDayInterval);
            await _context.SaveChangesAsync();
        }
    }
}