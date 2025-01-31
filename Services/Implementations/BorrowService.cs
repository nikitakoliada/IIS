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
        var startDate = DateTime.Now;

        var borrows = (await borrowRepository.GetAllWithIncludesAsync())
            .Where(x => x.State != BorrowState.Rejected && x.ToDate > startDate).OrderBy(x => x.FromDate);

        var freeIntervals = new List<Interval>();

        foreach (var borrow in borrows)
        {
            if (borrow.FromDate < startDate)
            {
                startDate = borrow.ToDate + TimeSpan.FromDays(1);
                continue;
            }

            var freeIntervalEnd = borrow.FromDate - TimeSpan.FromDays(1);

            if ((freeIntervalEnd - startDate).Days >= 0)
            {
                freeIntervals.Add((startDate, freeIntervalEnd));
            }

            startDate = borrow.ToDate + TimeSpan.FromDays(1);
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
        return !(await borrowRepository.GetByEquipmentId(equipmentId)).Where(x => x.State != BorrowState.Rejected).Any(
            x =>
                (x.FromDate <= interval.From && interval.From <= x.ToDate) ||
                (x.FromDate <= interval.To && interval.To <= x.ToDate));
    }
}