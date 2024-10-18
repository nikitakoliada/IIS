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
using Microsoft.AspNetCore.Identity;

namespace IIS.Controllers
{
    public class StudioController(StudioRepository studioRepository, UserManager<User> userManager)
        : Controller
    {
        // GET: Studio
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString)
        {
            var studios = await studioRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                studios = [..studios.Where(s => 
                    s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))];
            }
            
            return View(studios);
        }

        // GET: Studio/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            var currentUserRoles = await userManager.GetRolesAsync(currentUser);
            Studio studio;

            if (id == null)
            {
                if (currentUser.AssignedStudioId == null)
                {
                    return NotFound();
                }
                
                studio = await studioRepository.GetByIdAsync(currentUser.AssignedStudioId.Value);
            }
            else
            {
                if (!currentUserRoles.Contains("Admin"))
                {
                    return Forbid();
                }
                
                studio = await studioRepository.GetByIdAsync(id.Value);
                if (studio == null)
                {
                    return NotFound();
                }
            }

            return View(studio);
        }

        // GET: Studio/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Studio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Studio studio)
        {
            if (!ModelState.IsValid)
            {
                return View(studio);
            }
            
            await studioRepository.CreateAsync(studio);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Studio/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await studioRepository.GetByIdAsync(id.Value);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: Studio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
                await studioRepository.UpdateAsync(studio);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await studioRepository.GetByIdAsync(studio.Id) == null)
                {
                    return NotFound();
                }
                
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Studio/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await studioRepository.GetByIdAsync(id.Value);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: Studio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studio = await studioRepository.GetByIdAsync(id);
            if (studio == null)
            {
                return NotFound();
            }
            
            await studioRepository.RemoveAsync(studio);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
