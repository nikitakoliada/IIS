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
    public class EquipmentController : Controller
    {
        private readonly EquipmentTypeRepository _equipmentTypeRepository;
        private readonly StudioRepository _studioRepository;
        private readonly EquipmentRepository _equipmentRepository;
        
        public EquipmentController(EquipmentTypeRepository equipmentTypeRepository, StudioRepository studioRepository, EquipmentRepository equipmentRepository)
        {
            _equipmentTypeRepository = equipmentTypeRepository;
            _studioRepository = studioRepository;
            _equipmentRepository = equipmentRepository;
        }

        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            return View(await _equipmentRepository.GetAllWithIncludesAsync());
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _equipmentRepository.GetByIdWithIncludesAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }
            
            return View(equipment);
        }

        // GET: Equipment/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EquipmentTypeId"] = new SelectList(await _equipmentTypeRepository.GetAllAsync(), "Id", "Name");
            ViewData["StudioId"] = new SelectList(await _studioRepository.GetAllAsync(), "Id", "Name");
            
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ManufactureYear,PurchaseDate,Image,MaxRentalTime,StudioId,EquipmentTypeId")] Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EquipmentTypeId"] = new SelectList(await _equipmentTypeRepository.GetAllAsync(), "Id", "Name");
                ViewData["StudioId"] = new SelectList(await _studioRepository.GetAllAsync(), "Id", "Name");
                
                return View(equipment);
            }
            
            await _equipmentRepository.CreateAsync(equipment);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _equipmentRepository.GetByIdAsync(id.Value);
            if (equipment == null)
            {
                return NotFound();
            }
            
            ViewData["EquipmentTypeId"] = new SelectList(await _equipmentTypeRepository.GetAllAsync(), "Id", "Name");
            ViewData["StudioId"] = new SelectList(await _studioRepository.GetAllAsync(), "Id", "Name");
            
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
                ViewData["EquipmentTypeId"] = new SelectList(await _equipmentTypeRepository.GetAllAsync(), "Id", "Name");
                ViewData["StudioId"] = new SelectList(await _studioRepository.GetAllAsync(), "Id", "Name");
                
                return View(equipment);
            }

            try
            {
                await _equipmentRepository.UpdateAsync(equipment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _equipmentRepository.GetByIdAsync(equipment.Id) == null) // TODO: Check if this is correct
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

            var equipment = await _equipmentRepository.GetByIdWithIncludesAsync(id.Value);
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
            var equipment = await _equipmentRepository.GetByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            await _equipmentRepository.RemoveAsync(equipment);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
