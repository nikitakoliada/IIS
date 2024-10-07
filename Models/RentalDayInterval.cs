namespace IIS.Models;

public class RentalDayInterval
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Place { get; set; }
    
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
}