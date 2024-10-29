using IIS.Enums;
using IIS.Models;

namespace IIS.Areas.Student.ViewModels;

public class ListBorrowViewModel
{
    public int Id { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public BorrowState State { get; set; }

    public static ListBorrowViewModel FromBorrowModel(Borrow model) => new ListBorrowViewModel()
    {
        Id = model.Id,
        EquipmentName = model.Equipment.Name,
        FromDate = model.FromDate,
        ToDate = model.ToDate,
        State = model.State
    };
}