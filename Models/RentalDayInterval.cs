using System.ComponentModel;

namespace IIS.Models;

public class RentalDayInterval
{
    public int Id { get; set; }
    [DisplayName("Day of the Week")]
    public required DayOfWeek DayOfWeek { get; set; }
    [DisplayName("Start Time")]
    public required TimeOnly StartTime { get; set; }
    [DisplayName("End Time")]
    public required TimeOnly EndTime { get; set; }
    [DisplayName("Rental Place")]
    public required string Place { get; set; }

    public required int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}