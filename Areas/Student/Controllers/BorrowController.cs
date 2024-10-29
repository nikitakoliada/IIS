using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IIS.Areas.Student.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IIS.Data;
using IIS.Enums;
using IIS.Models;
using IIS.Repositories;
using IIS.Services.Abstractions;

namespace IIS.Areas.Student.Controllers
{
    [Area("Student")]
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BorrowRepository borrowRepository;
        private readonly EquipmentRepository equipmentRepository;
        private readonly IBorrowService borrowService;

        public BorrowController(ApplicationDbContext context, BorrowRepository borrowRepository,
            IBorrowService borrowService, EquipmentRepository equipmentRepository)
        {
            _context = context;
            this.borrowRepository = borrowRepository;
            this.borrowService = borrowService;
            this.equipmentRepository = equipmentRepository;
        }

        // GET: Student/Borrow
        public async Task<IActionResult> Index()
        {
            var borrows = await borrowRepository.GetByUserId(GetUserId());
            return View(borrows.Select(ListBorrowViewModel.FromBorrowModel).ToList());
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

        // GET: Student/Borrow/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EquipmentId"] = new SelectList(await equipmentRepository.GetAllWithIncludesAsync(), "Id", "Name");
            return View();
        }

        // POST: Student/Borrow/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBorrowViewModel borrow)
        {
            if (ModelState.IsValid)
            {
                if (!await borrowService.IsEquipmentAvailable(borrow.EquipmentId, (borrow.FromDate, borrow.ToDate)))
                {
                    var closestFreeInterval =
                        await borrowService.FindClosestFreeInterval(borrow.EquipmentId, borrow.FromDate);

                    ModelState.AddModelError("",
                        $"The selected interval is occupied. The closest interval is " +
                        $"from {closestFreeInterval.From.ToShortDateString()} to {closestFreeInterval.To.ToShortDateString()}");

                    return View(borrow);
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

            ViewData["EquipmentId"] = new SelectList(await equipmentRepository.GetAllWithIncludesAsync(), "Id", "Name");
            return View(borrow);
        }

        // // GET: Student/Borrow/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var borrow = await _context.Reservations.FindAsync(id);
        //     if (borrow == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", borrow.EquipmentId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrow.UserId);
        //     return View(borrow);
        // }
        //
        // // POST: Student/Borrow/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id,
        //     [Bind("Id,FromDate,ToDate,State,UserId,EquipmentId")]
        //     Borrow borrow)
        // {
        //     if (id != borrow.Id)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(borrow);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!BorrowExists(borrow.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //
        //     ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", borrow.EquipmentId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrow.UserId);
        //     return View(borrow);
        // }

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

        private bool BorrowExists(int id) => _context.Reservations.Any(e => e.Id == id);

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}