using Microsoft.AspNetCore.Mvc;

namespace IIS.Models;

public class UserEditViewModel
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }
    
    public bool? IsAssignedToMyStudio { get; set; }
    public bool? IsStudioAdmin { get; set; }
    public bool? IsTeacher { get; set; }
}