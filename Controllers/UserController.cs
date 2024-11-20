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

            if (roles.Contains("StudioAdmin"))
            {
                ViewData["ShowStudioFilter"] = true;
            }

            if (roles.Contains("Admin"))
            {
                users = await userRepository.GetAllWithIncludesAsync();
                ViewData["IsAdmin"] = true;
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
        [Authorize]
        public async Task<IActionResult> Details(string? id)
        {
            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!CanLoggedInUserManageGivenUser(currentUser, user))
            {
                return Forbid();
            }

            return View(user);
        }

        // GET: User/Edit/5
        [Authorize(Roles = "Admin,StudioAdmin,Teacher")]
        public async Task<IActionResult> Edit(string? id)
        {
            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            var currentUserRoles = userManager.GetRolesAsync(currentUser!).Result;
            
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            if (!CanLoggedInUserManageGivenUser(currentUser, user))
            {
                return Forbid();
            }
            
            var userRoles = userManager.GetRolesAsync(user).Result;
            
            ViewData["IsAdminEditing"] = currentUserRoles.Contains("Admin");
            
            if (currentUserRoles.Contains("Admin"))
            {
                ViewData["IsStudioAdmin"] = userRoles.Contains("StudioAdmin");
            }
            if (currentUserRoles.Contains("StudioAdmin"))
            {
                ViewData["IsTeacher"] = userRoles.Contains("Teacher");
                ViewData["IsAssignedToMyStudio"] = user.AssignedStudioId == currentUser.AssignedStudioId;
            }
            
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id,
            [Bind("Id,Name,Address,BirthDate,IsAssignedToMyStudio,IsStudioAdmin,IsTeacher")] UserEditViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit), new { id });
            }

            User? foundUser;
            
            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            var currentUserRoles = userManager.GetRolesAsync(currentUser).Result;

            try
            {
                foundUser = await UpdateFromEditViewModel(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userRepository.ExistsAsync(user.Id))
                {
                    return NotFound();
                }
                
                throw;
            }
            
            if (currentUserRoles.Contains("Admin"))
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
            if (currentUserRoles.Contains("StudioAdmin"))
            {
                if (user.IsTeacher.HasValue && user.IsTeacher.Value)
                {
                    await userManager.AddToRoleAsync(foundUser, "Teacher");
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(foundUser, "Teacher");
                }
                //
                // var userModel = await userRepository.GetByIdAsync(user.Id);
                // if (userModel!.AssignedStudioId != null || userModel.AssignedStudioId != currentUser.AssignedStudioId)
                // {
                //     return NotFound();
                // }
                //
                // if (user.IsAssignedToMyStudio.HasValue && user.IsAssignedToMyStudio.Value)
                // {
                //     userModel.AssignedStudio = currentUser.AssignedStudio;
                //     await userManager.AddToRoleAsync(userModel, "Student");
                // }
                // else
                // {
                //     userModel.AssignedStudio = null;
                //     await userManager.RemoveFromRoleAsync(userModel, "Student");
                //     await userManager.RemoveFromRoleAsync(userModel, "Teacher");
                //     await userManager.RemoveFromRoleAsync(userModel, "StudioAdmin");
                // }
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
            
            await userRepository.UpdateAsync(user);
            
            return user;
        }

        private bool CanLoggedInUserManageGivenUser(User loggedInUser, User givenUser)
        {
            var loggedInUserRoles = userManager.GetRolesAsync(loggedInUser).Result;

            if (loggedInUserRoles.Contains("Admin"))
            {
                return true;
            }
            else if (loggedInUserRoles.Contains("StudioAdmin"))
            {
                return givenUser.AssignedStudio == loggedInUser.AssignedStudio || givenUser.AssignedStudio == null;
            }
            else if (loggedInUserRoles.Contains("Teacher"))
            {
                return givenUser.AssignedStudio == loggedInUser.AssignedStudio;
            }

            return loggedInUser == givenUser;
        }
    }
}
