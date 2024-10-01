using Microsoft.AspNetCore.Identity;

namespace IIS.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }

    public ICollection<Studio> StudiosAssigned { get; set; }
    public ICollection<Borrow> BorrowedEquipment { get; set; }
}