using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Contexts;
using Store.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Register
        public IActionResult Register()
        {
            //check if already SignedIn
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                var User = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CreatedAt = DateTime.Now,
                };
                var Result = await _userManager.CreateAsync(User,model.Password);
                if (Result.Succeeded) {
                    //successfully user registration
                    await _userManager.AddToRoleAsync(User, "client");

                    //sign in the new user
                    await _signInManager.SignInAsync(User,false);
                    //If isPersistent is set to false, the authentication cookie will last only for the duration of the current browser session.Once the user closes the browser,
                    //    the session ends, and the user will need to log in again the next time they visit the site.

                    return RedirectToAction("Index","Home");
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty,error.Description);
                    }
                }
            }
            
            return View(model);
        }
        #endregion
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (flag) {
                        //login
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect Password");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is not Exsits");
                }
            }
            return View(model);
        }
        #region Login


        #endregion

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null) {
                return RedirectToAction("Index", "Home");
            }
            //manully mapping
            var MappedProfile = new ProfileViewModel()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,
            };
            return View(MappedProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid) {
                var appUser = await _userManager.GetUserAsync (User);
                if (appUser is null) return RedirectToAction("Index", "Home");

                //Update the user profile
                appUser.FirstName = model.FirstName;
                appUser.LastName = model.LastName;
                appUser.Email = model.Email;
                appUser.UserName = model.Email.Split('@')[0];
                appUser.PhoneNumber = model.PhoneNumber;
                appUser.Address = model.Address;

                var Result = await _userManager.UpdateAsync(appUser);
                if (Result.Succeeded)
                    ViewBag.SuccessMessage = "Profile updated successfully";
                else
                    ViewBag.ErrorMessage = "Unable to update the profile";
            }
            else
            {
                ViewBag.ErrorMessage = "Please fill all the required fields with values";
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordViewModel model)
        {
            if (ModelState.IsValid) {
                var appUser = await _userManager.GetUserAsync(User);
                if(appUser is null)return RedirectToAction("Index", "Home");

                //update password
                var result = await _userManager.ChangePasswordAsync(appUser, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded) ViewBag.SuccessMessage = "Password updated successfully!";
                else ViewBag.ErrorMessage = "Error " + result.Errors.First().Description;
                return View();

            }

            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
