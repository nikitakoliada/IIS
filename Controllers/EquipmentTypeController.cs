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
using Microsoft.AspNetCore.Authorization;

namespace IIS.Controllers
{
    public class EquipmentTypeController(EquipmentTypeRepository equipmentTypeRepository) : Controller
    {
        // GET: EquipmentType
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Index(string searchString)
        {
            var equipmentTypes = await equipmentTypeRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                equipmentTypes = [..equipmentTypes.Where(e => 
                    e.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))];
            }

            return View(equipmentTypes);
        }

        // GET: EquipmentType/Details/5
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }

            return View(equipmentType);
        }

        // GET: EquipmentType/Create
        [Authorize(Roles = "Admin,StudioAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipmentType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Create([Bind("Id,Name")] EquipmentType equipmentType)
        {
            if (!ModelState.IsValid)
            {
                return View(equipmentType);
            }
            
            await equipmentTypeRepository.CreateAsync(equipmentType);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: EquipmentType/Edit/5
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }
            
            return View(equipmentType);
        }

        // POST: EquipmentType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] EquipmentType equipmentType)
        {
            if (id != equipmentType.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(equipmentType);
            }
            
            try
            {
                await equipmentTypeRepository.UpdateAsync(equipmentType);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await equipmentTypeRepository.GetByIdAsync(equipmentType.Id) == null)
                {
                    return NotFound();
                }
                
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: EquipmentType/Delete/5
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }

            return View(equipmentType);
        }

        // POST: EquipmentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StudioAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipmentType = await equipmentTypeRepository.GetByIdAsync(id);
            if (equipmentType != null)
            {
                await equipmentTypeRepository.RemoveAsync(equipmentType);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
