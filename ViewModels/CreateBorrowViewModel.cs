using System.ComponentModel.DataAnnotations;

namespace IIS.ViewModels;

public class CreateBorrowViewModel : IValidatableObject
{
    public int EquipmentId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "The start date cannot be in the past.",
                new[] { nameof(FromDate) }
            );
        }
        
        if (ToDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "The end date cannot be in the past.",
                new[] { nameof(FromDate) }
            );
        }
        
        if (FromDate > ToDate)
        {
            yield return new ValidationResult(
                "The start date must be before the end date.",
                new[] { nameof(FromDate) }
            );
        }
    }
}