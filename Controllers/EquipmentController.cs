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

namespace IIS.Controllers
{
    public class EquipmentController(
        EquipmentTypeRepository equipmentTypeRepository,
        StudioRepository studioRepository,
        EquipmentRepository equipmentRepository)
        : Controller
    {
        // GET: Equipment
        public async Task<IActionResult> Index()
        {
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
            
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ManufactureYear,PurchaseDate,Image,MaxRentalTime,StudioId,EquipmentTypeId")] Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
                ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
                
                return View(equipment);
            }
            
            await equipmentRepository.CreateAsync(equipment);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await equipmentRepository.GetByIdAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }
            
            ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
            ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
            
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManufactureYear,PurchaseDate,Image,MaxRentalTime,StudioId,EquipmentTypeId")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                ViewData["EquipmentTypeId"] = new SelectList(await equipmentTypeRepository.GetAllAsync(), "Id", "Name");
                ViewData["StudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
                
                return View(equipment);
            }

            try
            {
                await equipmentRepository.UpdateAsync(equipment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await equipmentRepository.GetByIdAsync(equipment.Id) == null) // TODO: Check if this is correct
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
