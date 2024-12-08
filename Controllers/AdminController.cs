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
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                UserRepository.users = await DBHandler.GetUsersList();
                return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                HttpContext.Session.Remove("AdminEmail");
                HttpContext.Session.Remove("AdminPassword");
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                if (await DBHandler.DeleteUser(id))
                {
                    UserRepository.users = await DBHandler.GetUsersList();
                    return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Deletion Failed...");
                    return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
                }
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                User user = await DBHandler.GetUserById(id);
                return View("~/Views/Admin/UpdateUserView.cshtml", user);
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User u)
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                if (ModelState.IsValid)
                {
                    if (await DBHandler.UpdateUserByAdmin(u))
                    {
                        UserRepository.users = await DBHandler.GetUsersList();
                        ModelState.AddModelError(string.Empty, "Account Updated successfully...");
                        return View("~/Views/Admin/UpdateUserView.cshtml", u);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User Updation Failed...");
                        return View("~/Views/Admin/UpdateUserView.cshtml", u);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Enter Data in all Field...");
                    return View("~/Views/Admin/UpdateUserView.cshtml", u);
                }
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                return View("~/Views/Admin/CreateUserForm.cshtml");
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User u)
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                if (ModelState.IsValid)
                {
                    if (await DBHandler.RegisterUser(u))
                    {
                        UserRepository.users = await DBHandler.GetUsersList();
                        return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Registration Failed...");
                        return View("~/Views/Admin/CreateUserForm.cshtml");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please Enter Data in all Fields...");
                    return View("~/Views/Admin/CreateUserForm.cshtml");
                }
            }
            else
            {
                return View("~/Views/Home/AdminLoginForm.cshtml");
            }
        }
    }
}
