using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using exam.Models;
using System.Linq;

namespace exam.Controllers {
    public class UserController : Controller {
        private ExamContext _context;
        public UserController(ExamContext context) {
            _context = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index() {
            if (HttpContext.Session.GetInt32("UserId") > 0) {
                TempData["message"] = "You're already logged in, can't skip to Home Page";
                return RedirectToAction("Activity", "Activity");
            }
            ViewBag.Errors = new List<string>();
            ViewBag.Message = TempData["message"];
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model) {
            if (_context.users.Where(u => u.email == model.email).SingleOrDefault() != null) {
                TempData["message"] = "Email is taken!";
                return RedirectToAction("Index");
            }
            else {
                if(ModelState.IsValid) {
                    User NewUser = new User {
                        first_name = model.first_name,
                        last_name = model.last_name,
                        email = model.email,
                        password = model.password
                    };
                    NewUser.created_at = DateTime.Now;
                    NewUser.updated_at = DateTime.Now;
                    _context.Add(NewUser);
                    _context.SaveChanges();
                    User current = _context.users.Where(u => u.email == model.email).SingleOrDefault();
                    HttpContext.Session.SetInt32("UserId", current.id);
                    TempData["message"] = "You have successfully registered!";
                    return RedirectToAction("Activity","Activity");
                } 
                ViewBag.Errors = ModelState.Values;
                ViewBag.Status = true;
                return View("Index");
            }
        }

        [Route("signin")]
        public IActionResult Signin(string email, string password) {
            User current = _context.users.Where(u => u.email == email).SingleOrDefault();
            if (current != null) {
                // if(ModelState.IsValid) {
                if (current.password == password) {
                    HttpContext.Session.SetInt32("UserId", current.id);
                    return RedirectToAction("Activity","Activity");
                }
            }
            TempData["message"] = "Invalid Login!";
            return RedirectToAction("Index");
        }

        [Route("logout")]
        public IActionResult Logout() {
            if (HttpContext.Session.GetInt32("UserId") > 0) {
                HttpContext.Session.Clear();
                TempData["message"] = "Successfully logged out";
            } else {
                TempData["message"] = "No one to logout";
            }
            return RedirectToAction("Index");
        }
    }
}
