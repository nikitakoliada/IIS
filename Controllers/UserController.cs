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
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var roles = userManager.GetRolesAsync(currentUser!).Result;

            List<User> users;

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
            if (id == null)
            {
                return NotFound();
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            var currentUserRoles = userManager.GetRolesAsync(currentUser).Result;

            if (!CanLoggedInUserManageGivenUser(currentUser, user))
            {
                return Forbid();
            }

            var userRoles = userManager.GetRolesAsync(user).Result;

            if (currentUserRoles.Contains("Admin"))
            {
                ViewData["IsAdminEditing"] = true;
                ViewData["IsStudioAdmin"] = userRoles.Contains("StudioAdmin");
                ViewData["IsTeacher"] = userRoles.Contains("Teacher");
                ViewData["IsStudent"] = userRoles.Contains("Student");
                ViewData["AssignedStudioId"] = new SelectList(await studioRepository.GetAllAsync(), "Id", "Name");
            }
            else if (currentUserRoles.Contains("StudioAdmin"))
            {
                ViewData["IsTeacher"] = userRoles.Contains("Teacher");
                ViewData["IsAssignedToMyStudio"] = user.AssignedStudioId == currentUser.AssignedStudioId;
            }
            else if (currentUserRoles.Contains("Teacher"))
            {
                ViewData["IsStudent"] = userRoles.Contains("Student");
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StudioAdmin,Teacher")]
        public async Task<IActionResult> Edit(string id, UserEditViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit), new { id });
            }

            var foundUser = await userRepository.GetByIdAsync(id);
            if (foundUser == null)
            {
                return NotFound();
            }

            var currentUser = (await userManager.GetUserAsync(HttpContext.User))!;
            var currentUserRoles = userManager.GetRolesAsync(currentUser).Result;

            // !ADMIN: cannot change user details and assign other studio for user
            if (!currentUserRoles.Contains("Admin"))
            {
                user.Name = foundUser.Name;
                user.Address = foundUser.Address;
                user.BirthDate = foundUser.BirthDate;
                user.AssignedStudioId = null;
            }

            // STUDIOADMIN: assign to ones studio
            if (currentUserRoles.Contains("StudioAdmin") && !currentUserRoles.Contains("Admin"))
            {
                if (foundUser.AssignedStudioId != null && foundUser.AssignedStudioId != currentUser.AssignedStudioId)
                {
                    return NotFound();
                }

                if (user.IsAssignedToMyStudio.HasValue && user.IsAssignedToMyStudio.Value)
                {
                    user.AssignedStudioId = currentUser.AssignedStudioId;
                    await userManager.AddToRoleAsync(foundUser, "Student");
                }
                else
                {
                    user.AssignedStudioId = 0;
                    await userManager.RemoveFromRoleAsync(foundUser, "Student");
                }
            }

            // ADMIN: make StudioAdmin
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

            // ADMIN & STUDIOADMIN: make teachers
            if (currentUserRoles.Contains("StudioAdmin") || currentUserRoles.Contains("Admin"))
            {
                if (user.IsTeacher.HasValue && user.IsTeacher.Value)
                {
                    await userManager.AddToRoleAsync(foundUser, "Teacher");
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(foundUser, "Teacher");
                }
            }

            // ADMIN & TEACHER: make students
            if (currentUserRoles.Contains("Teacher") || currentUserRoles.Contains("Admin"))
            {
                if (user.IsStudent.HasValue && user.IsStudent.Value)
                {
                    await userManager.AddToRoleAsync(foundUser, "Student");
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(foundUser, "Student");
                }
            }

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

            user.Name = viewModel.Name ?? user.Name;
            user.Address = viewModel.Address ?? user.Address;
            user.BirthDate = viewModel.BirthDate ?? user.BirthDate;
            if (viewModel.AssignedStudioId == 0)
            {
                user.AssignedStudioId = null;
                await userManager.RemoveFromRoleAsync(user, "StudioAdmin");
                await userManager.RemoveFromRoleAsync(user, "Teacher");
                await userManager.RemoveFromRoleAsync(user, "Student");
            }
            else if (viewModel.AssignedStudioId != null && await studioRepository.GetByIdAsync(viewModel.AssignedStudioId.Value) != null)
            {
                user.AssignedStudioId = viewModel.AssignedStudioId;
                await userManager.AddToRoleAsync(user, "Student");
            }

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
                return givenUser.AssignedStudioId == loggedInUser.AssignedStudioId || givenUser.AssignedStudioId == null;
            }
            else if (loggedInUserRoles.Contains("Teacher"))
            {
                return givenUser.AssignedStudioId == loggedInUser.AssignedStudioId;
            }

            return loggedInUser == givenUser;
        }
    }
}
