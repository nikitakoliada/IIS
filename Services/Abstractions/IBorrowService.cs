using Interval = (System.DateTime From, System.DateTime To);

namespace IIS.Services.Abstractions;

public interface IBorrowService
{
    Task<Interval> FindClosestFreeInterval(int equipmentId, DateTime date);
    Task<bool> TryReserveEquipment(int equipmentId, string userId, Interval desiredPeriod);
    Task<bool> IsEquipmentAvailable(int equipmentId, Interval interval);
}