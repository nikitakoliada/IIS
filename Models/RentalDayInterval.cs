namespace IIS.Models;

public class RentalDayInterval
{
    public required int Id { get; set; }
    public required DayOfWeek DayOfWeek { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required string Place { get; set; }

    public required int EquipmentId { get; set; }
    public required Equipment Equipment { get; set; }
}