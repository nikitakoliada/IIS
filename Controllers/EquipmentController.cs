using System;
using System.Collections.Generic;
using System.Linq;
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
        UserRepository userRepository,
        EquipmentRepository equipmentRepository,
        EquipmentTypeRepository equipmentTypeRepository,
        RentalDayIntervalRepository rentalDayIntervalRepository,
        StudioRepository studioRepository)
        : Controller
    {
        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            var currentUser = await studioRepository.GetUserWithStudioAsync(User.Identity.Name);
            if (User.IsInRole("Student"))
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
            ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
            ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
            var currentUser = await studioRepository.GetUserWithStudioAsync(User.Identity.Name);
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
            Console.WriteLine(model.RentalDayIntervals.Count);
            foreach (var interval in model.RentalDayIntervals)
            {
                var intervalModel = new RentalDayInterval
                {
                    DayOfWeek = interval.DayOfWeek,
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime,
                    Place = interval.Place,
                    EquipmentId = model.Id
                };
                await rentalDayIntervalRepository.CreateAsync(intervalModel);
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
                RentalDayIntervals = model.RentalDayIntervals.Select(interval => new RentalDayInterval
                {
                    DayOfWeek = interval.DayOfWeek,
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime,
                    Place = interval.Place,
                    EquipmentId = model.Id
                }).ToList()
            };

            await equipmentRepository.CreateAsync(equipment);

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
            var currentUser = await studioRepository.GetUserWithStudioAsync(User.Identity.Name);
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
                }).ToList(),
                UsersForbiddenToBorrow = equipment.UsersForbiddenToBorrow.Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name
                }).ToList()
            };

            ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name", equipment.EquipmentTypeId);
            ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name", equipment.StudioId);
            ViewData["UserId"] = new SelectList(await userRepository.GetAllAsync(), "Id", "Name");

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
                ViewData["UserId"] = new SelectList(await userRepository.GetAllAsync(), "Id", "Name");

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

                var existingForbiddenUsers = equipment.UsersForbiddenToBorrow.ToList();
                equipment.UsersForbiddenToBorrow.Clear();

                if (model.UsersForbiddenToBorrow != null) 
                {
                    foreach (var forbiddenUser in model.UsersForbiddenToBorrow)
                    {
                        var foundForbiddenUser = await userRepository.GetByIdAsync(forbiddenUser.Id);
                        if (foundForbiddenUser == null)
                        {
                            ViewData["TypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name", model.EquipmentTypeId);
                            ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name", model.StudioId);
                            ViewData["UserId"] = new SelectList(await userRepository.GetAllAsync(), "Id", "Name");

                            return View(model);
                        }
                        equipment.UsersForbiddenToBorrow.Add(foundForbiddenUser);
                    }
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
            var currentUser = await studioRepository.GetUserWithStudioAsync(User.Identity.Name);
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