using IIS.Enums;
using IIS.Models;
using IIS.Repositories;
using IIS.Services.Abstractions;
using Interval = (System.DateTime From, System.DateTime To);

namespace IIS.Services.Implementations;

public class BorrowService : IBorrowService
{
    private readonly BorrowRepository borrowRepository;

    public BorrowService(BorrowRepository borrowRepository)
    {
        this.borrowRepository = borrowRepository;
    }

    public async Task<Interval> FindClosestFreeInterval(int equipmentId, DateTime date)
    {
        var borrows = (await borrowRepository.GetAllWithIncludesAsync()).OrderBy(x => x.FromDate);

        var startDate = DateTime.Now;

        var freeIntervals = new List<Interval>();
        
        foreach(var borrow in borrows)
        {
            if (startDate - borrow.FromDate > TimeSpan.FromDays(1))
            {
                freeIntervals.Add((startDate, borrow.FromDate));
            }

            startDate = borrow.ToDate;
        }

        freeIntervals.Add((startDate, DateTime.MaxValue));

        return freeIntervals.MinBy(x => Math.Min(Math.Abs((x.From - date).Ticks), Math.Abs((x.To - date).Ticks)));
    }

    public async Task<bool> TryReserveEquipment(int equipmentId, string userId, Interval desiredPeriod)
    {
        if (!await IsEquipmentAvailable(equipmentId, desiredPeriod))
        {
            return false;
        }

        await borrowRepository.CreateAsync(new Borrow()
        {
            FromDate = desiredPeriod.From,
            ToDate = desiredPeriod.To,
            EquipmentId = equipmentId,
            UserId = userId,
            State = BorrowState.Pending,
        });

        return true;
    }

    public async Task<bool> IsEquipmentAvailable(int equipmentId, Interval interval)
    {
        return !(await borrowRepository.GetAllWithIncludesAsync()).Any(x =>
            (x.FromDate <= interval.From && interval.From <= x.ToDate) ||
            (x.FromDate <= interval.To && interval.To <= x.ToDate));
    }
}