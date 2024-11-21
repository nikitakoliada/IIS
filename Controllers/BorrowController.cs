using System.Security.Claims;
using IIS.Data;
using IIS.Enums;
using IIS.Models;
using IIS.Repositories;
using IIS.Services.Abstractions;
using IIS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IIS.Controllers
{
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BorrowRepository borrowRepository;
        private readonly EquipmentRepository equipmentRepository;
        private readonly UserRepository userRepository;
        private readonly IBorrowService borrowService;

        public BorrowController(ApplicationDbContext context, BorrowRepository borrowRepository,
            IBorrowService borrowService, EquipmentRepository equipmentRepository, UserRepository userRepository)
        {
            _context = context;
            this.borrowRepository = borrowRepository;
            this.borrowService = borrowService;
            this.equipmentRepository = equipmentRepository;
            this.userRepository = userRepository;
        }

        // GET: Student/Borrow
        public async Task<IActionResult> Index()
        {
            List<Borrow> borrows;

            if (User.IsInRole("Student"))
            {
                borrows = await borrowRepository.GetByUserId(GetUserId());
            }
            else
            {
                var user = await userRepository.GetByIdAsync(GetUserId());
                borrows = await borrowRepository.GetByStudioId(user.AssignedStudioId.Value);
            }

            return View(borrows.Select(x => ListBorrowViewModel.FromBorrowModel(x, GetUserId())).ToList());
        }

        // GET: Student/Borrow/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Reservations
                .Include(b => b.Equipment)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // GET: Student/Borrow/Create/1
        public async Task<IActionResult> Create(int id)
        {
            var correspondingEquipment = await equipmentRepository.GetByIdWithIncludesAsync(id);
            var user = await userRepository.GetByIdAsync(GetUserId());
         
            if (correspondingEquipment == null)
            {
                ModelState.AddModelError("",
                    $"Equipment you are trying to borrow does not exist");
                return NotFound();
            }
            
            if (correspondingEquipment.UsersForbiddenToBorrow.Any(x => x.Id == GetUserId()) ||
                user.AssignedStudioId == null || user.AssignedStudioId != correspondingEquipment.StudioId)
            {
                return Forbid();
            }
            ViewData["EquipmentId"] = id;
            return View();
        }

        // POST: Student/Borrow/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBorrowViewModel borrow)
        {
            Task<IActionResult> ReturnView(CreateBorrowViewModel borrow)
            {
                ViewData["EquipmentId"] = borrow.EquipmentId;
                return Task.FromResult<IActionResult>(View(borrow));
            }

            if (ModelState.IsValid)
            {
                var correspondingEquipment = await equipmentRepository.GetByIdWithIncludesAsync(borrow.EquipmentId);
                var user = await userRepository.GetByIdAsync(GetUserId());
                if (correspondingEquipment == null)
                {
                    ModelState.AddModelError("",
                        $"Equipment you are trying to borrow does not exist");
                    return await ReturnView(borrow);
                }

                if (correspondingEquipment.UsersForbiddenToBorrow.Any(x => x.Id == GetUserId()) ||
                    user.AssignedStudioId == null || user.AssignedStudioId != correspondingEquipment.StudioId)
                {
                    ModelState.AddModelError("",
                        $"You are not allowed to borrow this equipment");
                    return await ReturnView(borrow);
                }


                if (correspondingEquipment.MaxRentalTime != null &&
                    borrow.ToDate - borrow.FromDate > correspondingEquipment.MaxRentalTime.Value)
                {
                    ModelState.AddModelError("",
                        $"Cannot make a reservation for more than {correspondingEquipment.MaxRentalTime.Value.Days} days");
                    return await ReturnView(borrow);
                }

                if (!await borrowService.IsEquipmentAvailable(borrow.EquipmentId, (borrow.FromDate, borrow.ToDate)))
                {
                    var closestFreeInterval =
                        await borrowService.FindClosestFreeInterval(borrow.EquipmentId, borrow.FromDate);

                    ModelState.AddModelError("",
                        $"The selected interval is occupied. The closest interval is " +
                        $"from {closestFreeInterval.From.ToShortDateString()} {(closestFreeInterval.To != DateTime.MaxValue ? "to " + closestFreeInterval.To.ToShortDateString() : string.Empty)}");

                    return await ReturnView(borrow);
                }

                await borrowRepository.CreateAsync(new Borrow()
                {
                    EquipmentId = borrow.EquipmentId,
                    FromDate = borrow.FromDate,
                    ToDate = borrow.ToDate,
                    State = BorrowState.Pending,
                    UserId = GetUserId()
                });
                return RedirectToAction(nameof(Index));
            }

            return await ReturnView(borrow);
        }


        // GET: Student/Borrow/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await borrowRepository.GetByIdAsync(id.Value);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // POST: Student/Borrow/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrow = await borrowRepository.GetByIdAsync(id);

            if (borrow != null)
            {
                await borrowRepository.RemoveAsync(borrow);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Student/Borrow/Accept/5
        [HttpPost, ActionName("Accept")]
        public Task<IActionResult> AcceptBorrow(int id)
        {
            return ChangeState(id, BorrowState.Pending, BorrowState.Accepted);
        }

        // POST: Student/Borrow/Reject/5
        [HttpPost, ActionName("Reject")]
        public Task<IActionResult> RejectBorrow(int id)
        {
            return ChangeState(id, BorrowState.Pending, BorrowState.Rejected);
        }

        // POST: Student/Borrow/Returned/5
        [HttpPost, ActionName("Returned")]
        public Task<IActionResult> BorrowReturned(int id)
        {
            return ChangeState(id, BorrowState.Accepted, BorrowState.Returned);
        }

        private async Task<IActionResult> ChangeState(int id, BorrowState requiredState, BorrowState newState)
        {
            var borrow = await borrowRepository.GetByIdAsync(id);

            if (borrow == null)
            {
                return NotFound("");
            }

            if (borrow.State != requiredState)
            {
                return BadRequest();
            }

            borrow.State = newState;
            await borrowRepository.UpdateAsync(borrow);

            return NoContent();
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}