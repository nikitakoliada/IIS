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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IIS.Controllers
{
    public class UserController(UserRepository userRepository, StudioRepository studioRepository, UserManager<User> userManager)
        : Controller
    {
        // GET: User
        [Authorize(Roles = "Admin,StudioAdmin,Teacher")]
        public async Task<IActionResult> Index(string searchString)
        {
            List<User> users;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var roles = userManager.GetRolesAsync(currentUser!).Result;

            if (roles.Contains("Admin"))
            {
                users = await userRepository.GetAllWithIncludesAsync();
            }
            else if (roles.Contains("StudioAdmin"))
            {
                users = await userRepository.GetAllFromAndWithoutStudioWithIncludesFromStudioAsync(currentUser!.AssignedStudioId.Value);
            }
            else
            {
                users = await userRepository.GetAllFromStudioWithIncludesFromStudioAsync(currentUser!.AssignedStudioId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                users = [..users.Where(u => 
                    u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))];
            }
            
            return View(users);
        }

        // GET: User/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            ViewData["AssignedStudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
            if (await IsCurrentUserAdmin())
            {
                ViewData["IsStudioAdmin"] = IsUserStudioAdmin((await userRepository.GetByIdAsync(id))!);
            }
            
            
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id,
            [Bind("Id,Name,Address,BirthDate,AssignedStudioId,IsAdmin,IsStudioAdmin")] UserEditViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["AssignedStudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
                if (await IsCurrentUserAdmin())
                {
                    ViewData["IsStudioAdmin"] = IsUserStudioAdmin((await userRepository.GetByIdAsync(id))!);
                }
                
                return View(await userRepository.GetByIdAsync(user.Id));
            }

            User? foundUser;

            try
            {
                foundUser = await UpdateFromEditViewModel(user);
            }
            catch (DbUpdateConcurrencyException) // TODO: check if this condition is needed
            {
                if (!await userRepository.ExistsAsync(user.Id))
                {
                    return NotFound();
                }
                
                throw;
            }
            
            if (await IsCurrentUserAdmin())
            {
                if (user.IsStudioAdmin.HasValue && user.IsStudioAdmin.Value)
                {
                    await userManager.AddToRoleAsync(foundUser, "StudioAdmin");
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(foundUser, "StudioAdmin");
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            await userRepository.RemoveAsync(user);
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<User?> UpdateFromEditViewModel(UserEditViewModel viewModel)
        {
            var user = await userRepository.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return null;
            }
            
            user.Name = viewModel.Name;
            user.Address = viewModel.Address;
            user.BirthDate = viewModel.BirthDate;
            user.AssignedStudioId = viewModel.AssignedStudioId;
            
            Console.WriteLine("1");
            
            await userRepository.UpdateAsync(user);
            
            Console.WriteLine("2");

            return user;
        }
        
        private async Task<bool> IsCurrentUserAdmin()
        {
            return userManager.GetRolesAsync((
                await userManager.GetUserAsync(HttpContext.User))!).Result.Contains("Admin");
        }
        
        private bool IsUserStudioAdmin(User user)
        {
            return userManager.GetRolesAsync(user).Result.Contains("StudioAdmin");
        }
    }
}
