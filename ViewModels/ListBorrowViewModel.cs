using IIS.Enums;
using IIS.Models;

namespace IIS.ViewModels;

public class ListBorrowViewModel
{
    public int Id { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; }
    public DateTime FromDate { get; set; }
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