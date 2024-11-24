using System.ComponentModel;
using IIS.Enums;
using IIS.Models;

namespace IIS.ViewModels;

public class ListBorrowViewModel
{
    public int Id { get; set; }
    [DisplayName("Equipment")]
    public string EquipmentName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    [DisplayName("User Email")]
    public string UserEmail { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; }
    [DisplayName("Start Date")]
    public DateTime FromDate { get; set; }
    [DisplayName("End Date")]
    public DateTime ToDate { get; set; }
    public BorrowState State { get; set; }

    public static ListBorrowViewModel FromBorrowModel(Borrow model, string userId) => new ListBorrowViewModel()
    {
        Id = model.Id,
        EquipmentName = model.Equipment.Name,
        FromDate = model.FromDate,
        ToDate = model.ToDate,
        State = model.State,
        UserId = model.UserId,
        UserEmail = model.User.Email!,
        CanBeDeleted = userId == model.UserId && model.State == BorrowState.Pending
    };
}