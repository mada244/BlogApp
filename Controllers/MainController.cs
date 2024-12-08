using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;
using Microsoft.Extensions.Hosting;

namespace BlogApp.Controllers
{
    public class MainController : Controller
    {
        private readonly IWebHostEnvironment Environment;
        public MainController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        public async Task<ViewResult> Index()
        {
            PostRepository.posts = await DBHandler.GetPosts();
            return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
        }

        [HttpGet]
        public ViewResult CreateNewPost()
        {
            return View("~/Views/Main/CreatePostView.cshtml");
        }

        [HttpPost]
        public async Task<ViewResult> CreateNewPost(Post p)
        {
            if (ModelState.IsValid)
            {
                p.UserId = HttpContext.Session.GetString("UserId");
                p.UserEmail = HttpContext.Session.GetString("UserEmail");
                p.UserProfilePicture = HttpContext.Session.GetString("UserProfilePicture");

                if (await DBHandler.AddPost(p))
                {
                    return View("~/Views/Main/PostView.cshtml", p);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Post Creation Failed...");
                    return View("~/Views/Main/CreatePostView.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please Enter Correct Data");
                return View("~/Views/Main/CreatePostView.cshtml");
            }
        }

        [HttpGet]
        public ViewResult EditPost(int id)
        {
            Post p = PostRepository.posts.Find(s => s.Id == id.ToString());
            return View("~/Views/Main/EditPostView.cshtml", p);
        }

        [HttpPost]
        public async Task<ViewResult> EditPost(Post ps)
        {
            if (ModelState.IsValid)
            {
                if (await DBHandler.UpdatePost(ps))
                {
                    PostRepository.posts = await DBHandler.GetPosts();
                    Post p = PostRepository.posts.Find(s => s.Id == ps.Id);
                    return View("~/Views/Main/PostView.cshtml", p);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Post Updating Failed...");
                    return View("~/Views/Main/EditPostView.cshtml", ps);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enter Correct Data...");
                return View("~/Views/Main/EditPostView.cshtml", ps);
            }
        }

        public ViewResult PostView(int id)
        {
            Post p = PostRepository.posts.Find(s => s.Id == id.ToString());
            return View("~/Views/Main/PostView.cshtml", p);
        }

        public async Task<ViewResult> DeletePost(string id)
        {
            if (await DBHandler.DeletePost(id))
            {
                PostRepository.posts = await DBHandler.GetPosts();
                return View("~/Views/Main/MainView.cshtml", PostRepository.posts);
            }
            else
            {
                Post p = PostRepository.posts.Find(s => s.Id == id.ToString());
                ModelState.AddModelError(string.Empty, "Post Deletion Failed...");
                return View("~/Views/Main/PostView.cshtml", p);
            }
        }

        public ViewResult Logout()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                HttpContext.Session.Remove("UserId");
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("UserEmail");
                HttpContext.Session.Remove("UserPassword");
                HttpContext.Session.Remove("UserProfilePicture");
            }
            return View("~/Views/Home/LoginForm.cshtml");
        }

        [HttpGet]
        public async Task<ViewResult> UpdateProfile()
        {
            User u = await DBHandler.GetUser(HttpContext.Session.GetString("UserEmail"));
            return View("~/Views/Main/ProfileView.cshtml", u);
        }

        [HttpPost]
        public async Task<ViewResult> UpdateProfile(User u, IFormFile profile_image, string newPassword)
        {
            if (ModelState.IsValid && newPassword != null && profile_image != null)
            {
                var obj = await DBHandler.ValidUser(u.Email, u.Password);
                User us = obj.Item2;

                if (obj.Item1)
                {
                    string wwwPath = this.Environment.WebRootPath;
                    string contentPath = this.Environment.ContentRootPath;

                    string path = Path.Combine(this.Environment.WebRootPath, "Images");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(profile_image.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await profile_image.CopyToAsync(stream);
                    }

                    if (await DBHandler.UpdateUser(u, newPassword, fileName))
                    {
                        us = await DBHandler.GetUser(u.Email);
                        return View("~/Views/Main/ProfileView.cshtml", us);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Post Updating Failed...");
                        return View("~/Views/Main/ProfileView.cshtml", u);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Enter Correct Old Password");
                    return View("~/Views/Main/ProfileView.cshtml", u);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enter Correct Data...");
                u = await DBHandler.GetUser(u.Email);
                return View("~/Views/Main/ProfileView.cshtml", u);
            }
        }
    }
}
