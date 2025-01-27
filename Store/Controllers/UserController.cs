using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.Models;

namespace Store.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users.OrderByDescending(u => u.CreatedAt).ToList();
            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return BadRequest();

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null) return RedirectToAction("Index");
            ViewBag.Roles = await _userManager.GetRolesAsync(appUser);

            ///get available roles
            var availableRoles = _roleManager.Roles.ToList();
            var items = new List<SelectListItem>();
            foreach (var role in availableRoles) {
                items.Add(
                    new SelectListItem
                    {
                        Text = role.NormalizedName,
                        Value = role.Name,
                        Selected = await _userManager.IsInRoleAsync(appUser,role.Name!),


                    });
            }
            ViewBag.SelectItems = items;
            return View(appUser);
        }


        public async Task<ActionResult> EditRole(string? id,string? newRole)
        {
            if(id == null || newRole == null) return BadRequest();
            var roleExists = await _roleManager.RoleExistsAsync(newRole);
            var appUser = await _userManager.FindByIdAsync(id);
            if(appUser == null || !roleExists)
            {
                return RedirectToAction("Index","User");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You Cannot update your own role!";
                return RedirectToAction("Index", "User", new { id });
            }
            ///update user role
            var userRoles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(appUser, role);
            }
            // Add the new role
            await _userManager.AddToRoleAsync(appUser,newRole);

            TempData["SuccessMessage"] = "User Role updated successfully";
            return RedirectToAction("Details","User",new { id });
        }

        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction("Details", "User", new { id });
            }

            // delete user account
            var result = await _userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users");
            }

            TempData["ErrorMessage"] = "Unable to delete this account: " + result.Errors.First().Description;
            return RedirectToAction("Details", "User", new { id });
        }
    }
}
