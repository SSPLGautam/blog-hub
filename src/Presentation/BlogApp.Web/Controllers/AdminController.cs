using BlogApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller   
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(UserManager<IdentityUser>userManager,RoleManager<IdentityRole> roleMangaer) 
        { 
        
           _userManager = userManager;
            _roleManager = roleMangaer;



        }
        public async Task<IActionResult> Users()
        {
            var viewModel = new List<UserViewModel>();
            var users = _userManager.Users
                .Where(u => u.Email != "admin@gmail.com")
                .ToList();

            foreach (var user in users)
            {
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    IsLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now
                };

                userViewModel.UserRoles = await _userManager.GetRolesAsync(user);

                viewModel.Add(userViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = userRoles.Except(roles ?? new List<string>());
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

          
            var rolesToAdd = (roles ?? new List<string>()).Except(userRoles);
            await _userManager.AddToRolesAsync(user, rolesToAdd);
            TempData["SuccessMessage"] = "Roles updated successfully for " + user.Email;
            return RedirectToAction("Users");
        }
        [HttpPost]
        public async Task<IActionResult> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            //TempData["SuccessMessage"] = "User locked successfully.";
            return RedirectToAction("Users");
        }
        [HttpPost]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);

            //TempData["SuccessMessage"] = "User unlocked successfully.";
            return RedirectToAction("Users");
        }
    }
}
