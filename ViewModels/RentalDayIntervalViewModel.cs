using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IIS.ViewModels;

public class RentalDayIntervalViewModel : IValidatableObject
{
    [DisplayName("Day of the Week")]
    [Required(ErrorMessage = "The day of the week is required.")]
    public DayOfWeek DayOfWeek { get; set; }

    [DisplayName("Start Time")]
    [Required(ErrorMessage = "The start time is required.")]
    public TimeOnly StartTime { get; set; }

    [DisplayName("End Time")]
    [Required(ErrorMessage = "The end time is required.")]
    public TimeOnly EndTime { get; set; }

    [DisplayName("Rental Place")]
    [Required(ErrorMessage = "The rental place is required.")]
    [StringLength(100, ErrorMessage = "The rental place name cannot exceed 100 characters.")]
    public string Place { get; set; } = string.Empty;
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Ensure StartTime is before EndTime
        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                "The start time must be before the end time.",
                new[] { nameof(StartTime), nameof(EndTime) }
            );
        }

        // Example of additional validation logic
        if (string.IsNullOrWhiteSpace(Place))
        {
            yield return new ValidationResult(
                "Rental place must be specified.",
                new[] { nameof(Place) }
            );
        }
    }
}