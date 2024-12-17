using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private async Task<bool> IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        public async Task<ViewResult> Index()
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            return View("~/Views/Home/LoginForm.cshtml");
        }

        public async Task<ViewResult> About()
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            return View("~/Views/Home/About.cshtml");
        }

       
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Please enter both email and password.");
                return View("~/Views/Home/LoginForm.cshtml");
            }

            var obj = await DBHandler.ValidUser(email, password);
            User u = obj.Item2;

            if (obj.Item1)
            {
                HttpContext.Session.SetString("UserId", u.Id);
                HttpContext.Session.SetString("UserName", u.Username);
                HttpContext.Session.SetString("UserEmail", u.Email);
                HttpContext.Session.SetString("UserPassword", u.Password);

                if (u.Role == "Admin")
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    HttpContext.Session.SetString("UserRole", "User");
                    PostRepository.posts = await DBHandler.GetPosts();
                    return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Account doesn't exist or credentials are incorrect.");
                return View("~/Views/Home/LoginForm.cshtml");
            }
        }

        [HttpGet]
        public async Task<ViewResult> SignupForm()
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            return View("~/Views/Home/SignupForm.cshtml");
        }

        [HttpPost]
        public async Task<ViewResult> SignupForm(User u)
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }

            if (ModelState.IsValid)
            {
                bool isRegistered = await DBHandler.RegisterUser(u);
                if (isRegistered)
                {
                    ModelState.AddModelError(string.Empty, "Registered Successfully...");
                    return View("~/Views/Home/LoginForm.cshtml");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Registration Failed. Try again.");
                    return View("~/Views/Home/SignupForm.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please enter correct data.");
                return View("~/Views/Home/SignupForm.cshtml");
            }
        }

        [HttpGet]
        public async Task<ViewResult> AdminLoginForm()
        {
            if (await IsUserLoggedIn())
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            return View("~/Views/Home/AdminLoginForm.cshtml");
        }
    }
}
