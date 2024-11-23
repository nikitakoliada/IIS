// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System;
using System.Threading.Tasks;
using IIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IIS.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public IndexModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string Username { get; set; }
        public string StatusMessage { get; set; }
        public bool IsEditMode { get; set; } = false;

        [BindProperty]
        public ViewModel Input { get; set; }
        
        public class ViewModel
        {
            public string? Name { get; set; }
            public string? Address { get; set; }
            public DateTime BirthDate { get; set; }
            public string? PhoneNumber { get; set; }

        }

        private async Task LoadAsync(User user)
        {
            Username = await _userManager.GetUserNameAsync(user);
            Input = new ViewModel
            {
                Name = user.Name,
                Address = user.Address,
                BirthDate = user.BirthDate,
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
            };
        }

        public async Task<IActionResult> OnGetAsync(string handler)
        {
            Input = new ViewModel(); // Initialize Input to prevent null reference
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IsEditMode = handler == "Edit";
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Input == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid input data.");
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.Name = Input.Name;
            user.Address = Input.Address;
            user.BirthDate = Input.BirthDate;
            user.PhoneNumber = Input.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error occurred while updating profile.";
                return RedirectToPage();
            }

            StatusMessage = "Your profile has been updated.";
            return RedirectToPage();
        }
    }
}