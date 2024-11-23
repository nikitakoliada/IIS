using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IIS.Data;
using IIS.Models;
using IIS.Repositories;
using IIS.ViewModels;

namespace IIS.Controllers
{
    public class EquipmentController(
        EquipmentTypeRepository equipmentTypeRepository,
        StudioRepository studioRepository,
        EquipmentRepository equipmentRepository,
        UserRepository userRepository,
        RentalDayIntervalRepository rentalDayIntervalRepository)
        : Controller
    {
        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            var currentUser = await userRepository.GetUserWithStudioAsync(User.Identity.Name);
            if (User.IsInRole("Student") || User.IsInRole("Teacher"))
            {
                int studioId = currentUser.AssignedStudioId.Value;
                if (studioId != null)
                {
                    return View(await equipmentRepository.GetByStudioIdAsync(studioId));
                }
                else if (currentUser.AssignedStudioId == null)
                {
                    return View(new List<Equipment>());
                }
            }
            return View(await equipmentRepository.GetAllWithIncludesAsync());
        }
        

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await equipmentRepository.GetByIdWithIncludesAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Create
        public async Task<IActionResult> Create()
        {
            if(User.IsInRole("Student") || User.IsInRole("StudioAdmin"))
            {
                return Forbid();
            }
            ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
            if (User.IsInRole("Teacher"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!; 
                ViewData["StudioId"] = new SelectList( new List<Studio>([ await studioRepository.GetByUserIdAsync(userId)]), "Id", "Name");
            }
            else
            {
                ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");

            }

            if (!(User.IsInRole("Teacher") || User.IsInRole("Admin")))
            {
                return Forbid();
            }
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
                ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
                return View(model);
            }
            
            // Map the view model to the main Equipment model
            var equipment = new Equipment
            {
                Name = model.Name,
                ManufactureYear = model.ManufactureYear,
                PurchaseDate = model.PurchaseDate,
                Image = model.Image,
                MaxRentalTime = model.MaxRentalDays == null ? null : TimeSpan.FromDays(model.MaxRentalDays.Value),
                StudioId = model.StudioId,
                EquipmentTypeId = model.EquipmentTypeId,
                OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!,
            };

            await equipmentRepository.CreateAsync(equipment);
            
            Console.WriteLine(model.RentalDayIntervals.Count);
            foreach (var interval in model.RentalDayIntervals)
            {
                var intervalModel = new RentalDayInterval
                {
                    DayOfWeek = interval.DayOfWeek,
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime,
                    Place = interval.Place,
                    EquipmentId = equipment.Id
                };
                await rentalDayIntervalRepository.CreateAsync(intervalModel);
            }
            
            return RedirectToAction(nameof(Index));
        }

       public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await equipmentRepository.GetByIdWithIncludesAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }
            var currentUser = await userRepository.GetUserWithStudioAsync(User.Identity.Name);
            if (!((currentUser?.AssignedStudioId == equipment.StudioId && User.IsInRole("Teacher")) || User.IsInRole("Admin")))
            {
                return Forbid();
            }
            // Prepare the view model
            var model = new EquipmentViewModel
            {
                Id = equipment.Id,
                Name = equipment.Name,
                ManufactureYear = equipment.ManufactureYear,
                PurchaseDate = equipment.PurchaseDate,
                Image = equipment.Image,
                MaxRentalDays = equipment.MaxRentalTime?.Days,
                StudioId = equipment.StudioId,
                EquipmentTypeId = equipment.EquipmentTypeId,
                RentalDayIntervals = equipment.RentalDayIntervals.Select(interval => new RentalDayIntervalViewModel
                {
                    DayOfWeek = interval.DayOfWeek,
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime,
                    Place = interval.Place
                }).ToList()
            };

            ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name", equipment.EquipmentTypeId);
            ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name", equipment.StudioId);

            return View(model);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipmentViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name", model.EquipmentTypeId);
                ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name", model.StudioId);
                return View(model);
            }

            try
            {
                // Update the main Equipment data
                var equipment = await equipmentRepository.GetByIdWithIncludesAsync(id);
                equipment.Name = model.Name;
                equipment.ManufactureYear = model.ManufactureYear;
                equipment.PurchaseDate = model.PurchaseDate;
                equipment.Image = model.Image;
                equipment.MaxRentalTime = model.MaxRentalDays == null ? null : TimeSpan.FromDays(model.MaxRentalDays.Value);
                equipment.StudioId = model.StudioId;
                equipment.EquipmentTypeId = model.EquipmentTypeId;

                // Update RentalDayIntervals
                var existingIntervals = equipment.RentalDayIntervals.ToList();
                equipment.RentalDayIntervals.Clear();

                // Delete removed intervals
                foreach (var interval in existingIntervals)
                {
                    await rentalDayIntervalRepository.DeleteAsync(interval.Id);
                }

                // Add updated/new intervals
                foreach (var interval in model.RentalDayIntervals)
                {
                    var intervalModel = new RentalDayInterval
                    {
                        DayOfWeek = interval.DayOfWeek,
                        StartTime = interval.StartTime,
                        EndTime = interval.EndTime,
                        Place = interval.Place,
                        EquipmentId = equipment.Id
                    };
                    equipment.RentalDayIntervals.Add(intervalModel);
                }

                await equipmentRepository.UpdateAsync(equipment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await equipmentRepository.GetByIdAsync(model.Id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Equipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await equipmentRepository.GetByIdWithIncludesAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }
            var currentUser = await userRepository.GetUserWithStudioAsync(User.Identity.Name);
            if (!((currentUser?.AssignedStudioId == equipment.StudioId && User.IsInRole("Teacher")) || User.IsInRole("Admin")))
            {
                return Forbid();
            }
            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await equipmentRepository.GetByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            await equipmentRepository.RemoveAsync(equipment);

            return RedirectToAction(nameof(Index));
        }
    }
}