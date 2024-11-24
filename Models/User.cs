using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace IIS.Models;

public class User : IdentityUser
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    
    [DisplayName("Birth Date")]
    public required DateTime BirthDate { get; set; }
    
    public int? AssignedStudioId { get; set; }
    
    [DisplayName("Assigned Studio")]
    public Studio? AssignedStudio { get; set; }

    public ICollection<Borrow> BorrowedEquipment { get; set; } = new List<Borrow>();
    public ICollection<Equipment> RestrictedEquipment { get; set; } = new List<Equipment>();
}