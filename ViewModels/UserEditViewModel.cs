using System;
using System.ComponentModel.DataAnnotations;
using IIS.Repositories;

namespace IIS.ViewModels;

public class UserEditViewModel
{
    public string Id { get; set; }
    
    public string? Name { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }

    [StudioExistsValidation(ErrorMessage = "The assigned studio does not exist.")]
    public int? AssignedStudioId { get; set; }
    public bool? IsAssignedToMyStudio { get; set; }
    
    public bool? IsAdmin { get; set; }
    public bool? IsStudioAdmin { get; set; }
    public bool? IsTeacher { get; set; }
    public bool? IsStudent { get; set; }
}

public class StudioExistsValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is int studioId)
        {
            var serviceProvider = validationContext.GetService(typeof(StudioRepository)) as StudioRepository;
            if (serviceProvider == null)
            {
                throw new InvalidOperationException("StudioRepository is not configured in the dependency injection container.");
            }

            if (studioId == 0)
            {
                return ValidationResult.Success;
            }

            var studio = Task.Run(async () => await serviceProvider.GetByIdAsync(studioId)).Result;
            if (studio == null)
            {
                return new ValidationResult(ErrorMessage ?? "The assigned studio does not exist.");
            }
        }

        return ValidationResult.Success;
    }
}
