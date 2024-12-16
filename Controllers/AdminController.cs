using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Both email and password are required.");
                return View("~/Views/Home/AdminLoginForm.cshtml");
            }

            var user = await DBHandler.ValidUser(email, password);
            if (user.Item1 && !string.IsNullOrEmpty(user.Item2.Role) && user.Item2.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("AdminEmail", email);
                HttpContext.Session.SetString("AdminRole", "Admin");

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password, or no admin privileges.");
                return View("~/Views/Home/AdminLoginForm.cshtml");
            }
        }

        public async Task<IActionResult> Index()
        {
            var adminEmail = HttpContext.Session.GetString("AdminEmail");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (!string.IsNullOrEmpty(adminEmail) && userRole == "Admin")
            {
                UserRepository.users = await DBHandler.GetUsersList();
                return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
            }

            return RedirectToAction("Login", "Admin");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session"); 
            TempData["SuccessMessage"] = "You have successfully logged out.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (await DBHandler.DeleteUser(id))
            {
                TempData["SuccessMessage"] = "User successfully deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete the user.";
            }

            UserRepository.users = await DBHandler.GetUsersList();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            User user = await DBHandler.GetUserById(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Admin");
            }

            return View("~/Views/Admin/UpdateUserView.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User u, string newPassword)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                User existingUser = await DBHandler.GetUserById(u.Id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Index", "Admin");
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    u.Password = existingUser.Password;
                }
                else
                {
                    u.Password = newPassword;
                }

                if (await DBHandler.UpdateUserByAdmin(u))
                {
                    UserRepository.users = await DBHandler.GetUsersList();
                    TempData["SuccessMessage"] = "Account updated successfully.";
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User update failed.");
                    return View("~/Views/Admin/UpdateUserView.cshtml", u);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields correctly.");
                return View("~/Views/Admin/UpdateUserView.cshtml", u);
            }
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            return View("~/Views/Admin/CreateUserForm.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User u)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                bool userRegistered = false;
                try
                {
                    userRegistered = await DBHandler.RegisterUser(u);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                    return View("~/Views/Admin/CreateUserForm.cshtml");
                }

                if (userRegistered)
                {
                    UserRepository.users = await DBHandler.GetUsersList();
                    TempData["SuccessMessage"] = "User registered successfully!";
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User registration failed.");
                    return View("~/Views/Admin/CreateUserForm.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields correctly.");
                return View("~/Views/Admin/CreateUserForm.cshtml");
            }
        }
    }
}
