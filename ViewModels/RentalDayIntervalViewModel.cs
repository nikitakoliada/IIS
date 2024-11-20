using System.ComponentModel;

namespace IIS.ViewModels;

public class RentalDayIntervalViewModel
{
    [DisplayName("Day of the Week")]
    public DayOfWeek DayOfWeek { get; set; }

    [DisplayName("Start Time")]
    public TimeOnly StartTime { get; set; }

    [DisplayName("End Time")]
    public TimeOnly EndTime { get; set; }

    [DisplayName("Rental Place")]
    public string Place { get; set; } = string.Empty;
}