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
    public class StudioController : Controller
    {
        private readonly StudioRepository _studioRepository;

        public StudioController(StudioRepository studioRepository)
        {
            _studioRepository = studioRepository;
        }

        // GET: Studio
        public async Task<IActionResult> Index(string searchString)
        {
            var studios = await _studioRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                studios = [..studios.Where(s => 
                    s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))];
            }
            
            return View(studios);
        }

        // GET: Studio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _studioRepository.GetByIdAsync(id.Value);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // GET: Studio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Studio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Studio studio)
        {
            if (!ModelState.IsValid)
            {
                return View(studio);
            }
            
            await _studioRepository.CreateAsync(studio);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Studio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _studioRepository.GetByIdAsync(id.Value);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: Studio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Studio studio)
        {
            if (id != studio.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(studio);
            }

            try
            {
                await _studioRepository.UpdateAsync(studio);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _studioRepository.GetByIdAsync(studio.Id) == null)
                {
                    return NotFound();
                }
                
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Studio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _studioRepository.GetByIdAsync(id.Value);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: Studio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studio = await _studioRepository.GetByIdAsync(id);
            if (studio == null)
            {
                return NotFound();
            }
            
            await _studioRepository.RemoveAsync(studio);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
