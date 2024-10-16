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
    public class EquipmentTypeController : Controller
    {
        private readonly EquipmentTypeRepository _equipmentTypeRepository;

        public EquipmentTypeController(EquipmentTypeRepository equipmentTypeRepository)
        {
            _equipmentTypeRepository = equipmentTypeRepository;
        }

        // GET: EquipmentType
        public async Task<IActionResult> Index()
        {
            return View(await _equipmentTypeRepository.GetAllAsync());
        }

        // GET: EquipmentType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await _equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }

            return View(equipmentType);
        }

        // GET: EquipmentType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipmentType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EquipmentType equipmentType)
        {
            if (!ModelState.IsValid)
            {
                return View(equipmentType);
            }
            
            await _equipmentTypeRepository.CreateAsync(equipmentType);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: EquipmentType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await _equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }
            
            return View(equipmentType);
        }

        // POST: EquipmentType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                await _equipmentTypeRepository.UpdateAsync(equipmentType);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _equipmentTypeRepository.GetByIdAsync(equipmentType.Id) == null)
                {
                    return NotFound();
                }
                
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: EquipmentType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentType = await _equipmentTypeRepository.GetByIdAsync(id.Value);
            if (equipmentType == null)
            {
                return NotFound();
            }

            return View(equipmentType);
        }

        // POST: EquipmentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipmentType = await _equipmentTypeRepository.GetByIdAsync(id);
            if (equipmentType != null)
            {
                await _equipmentTypeRepository.RemoveAsync(equipmentType);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
