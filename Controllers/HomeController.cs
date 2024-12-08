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
        public async Task<ViewResult> Index()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            return View("~/Views/Home/LoginForm.cshtml");
        }

        public async Task<ViewResult> About()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
                return View("~/Views/Home/About.cshtml");
        }

        [HttpPost]
        public async Task<ViewResult> Login(string email, string password)
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var obj = await DBHandler.ValidUser(email, password);
                    User u = obj.Item2;
                    if (obj.Item1)
                    {
                        HttpContext.Session.SetString("UserId", u.Id);
                        HttpContext.Session.SetString("UserName", u.Username);
                        HttpContext.Session.SetString("UserEmail", u.Email);
                        HttpContext.Session.SetString("UserPassword", u.Password);
                        //HttpContext.Session.SetString("UserProfilePicture", u.PictureFileName);
                        PostRepository.posts = await DBHandler.GetPosts();
                        return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account Doesn't Exist...");
                        return View("~/Views/Home/LoginForm.cshtml");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please Enter Correct Data");
                    return View("~/Views/Home/LoginForm.cshtml");
                }
            }
        }

        [HttpGet]
        public async Task<ViewResult> SignupForm()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
                return View("~/Views/Home/SignupForm.cshtml");
        }

        [HttpPost]
        public async Task<ViewResult> SignupForm(User u)
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (await DBHandler.RegisterUser(u))
                    {
                        ModelState.AddModelError(string.Empty, "Registered Successfully...");
                        return View("~/Views/Home/LoginForm.cshtml");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Registration Failed...");
                        return View("~/Views/Home/SignupForm.cshtml");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please Enter Correct Data");
                    return View("~/Views/Home/SignupForm.cshtml");
                }
            }
        }

        [HttpGet]
        public async Task<ViewResult> AdminLoginForm()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
                return View("~/Views/Home/AdminLoginForm.cshtml");
        }

        [HttpPost]
        public async Task<ViewResult> AdminLoginForm(string adminLoginEmail, string adminLoginPassword)
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
            {
                if (!string.IsNullOrEmpty(adminLoginEmail) && !string.IsNullOrEmpty(adminLoginPassword))
                {
                    if (adminLoginEmail == "admin@gmail.com" && adminLoginPassword == "admin")
                    {
                        HttpContext.Session.SetString("AdminEmail", adminLoginEmail);
                        HttpContext.Session.SetString("AdminPassword", adminLoginPassword);
                        UserRepository.users = await DBHandler.GetUsersList();
                        return View("~/Views/Admin/AdminMainView.cshtml", UserRepository.users);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Wrong Credentials...");
                        return View("~/Views/Home/AdminLoginForm.cshtml");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please Enter All Fields");
                    return View("~/Views/Home/AdminLoginForm.cshtml");
                }
            }
        }
    }
}
