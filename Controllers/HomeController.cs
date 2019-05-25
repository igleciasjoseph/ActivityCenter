using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ActivityCenter.Models;


namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private ACContext context;

        public HomeController(ACContext ac)
        {
            context = ac;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                var dbInUser = context.UserList.FirstOrDefault(u => u.Email == newUser.Email);
                if (dbInUser != null)
                {
                    ModelState.AddModelError("Email", "This Email is already in use");
                }
                else
                {
                    PasswordHasher<User> PWHasher = new PasswordHasher<User>();
                    newUser.Password = PWHasher.HashPassword(newUser, newUser.Password);
                    context.UserList.Add(newUser);
                    context.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", newUser.UserId);
                    return Redirect("/dashboard");
                }
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser User)
        {
            if (ModelState.IsValid)
            {
                var user = context.UserList.FirstOrDefault(u => u.Email == User.LoginEmail);
                if (user == null)
                {
                    ModelState.AddModelError("LoginEmail", "This Email doesn't exist, please Register");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(User, user.Password, User.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Incorrect Password, Please try again or register");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        return Redirect("/dashboard");
                    }
                }
            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }
            else
            {
                User User = context.UserList.FirstOrDefault(x => x.UserId == HttpContext.Session.GetInt32("UserId"));
                List<Occasion> Activities = context.ActList.Include(x => x.Coordinator).Include(y => y.Attendees).ThenInclude(z => z.SingleUser).ToList();
                ViewBag.Activities = Activities.OrderBy(z => z.Date);
                ViewBag.u = User;
                return View();
            }
        }

        [HttpGet("newactivity")]
        public IActionResult NewAct() {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("create")]
        public IActionResult NewActivity(Occasion o) {
            if (ModelState.IsValid)
            {
                Join NewJoin = new Join();
                o.UserID = (int)HttpContext.Session.GetInt32("UserId");
                context.ActList.Add(o);
                context.SaveChanges();


                NewJoin.OccasionId = o.OccasionId;
                NewJoin.UserId = o.UserID;
                context.Joinee.Add(NewJoin);
                context.SaveChanges();


                return Redirect($"activity/{o.OccasionId}");
            }
            else
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId == null)
                {
                    return Redirect("/");
                }
                else
                {
                    return View("NewAct");
                }
            }
        }

        [HttpGet("activity/{OccasionId}")]
        public IActionResult ActivityDisplay(int OccasionId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }
            else
            {
                User User = context.UserList.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
                Occasion Activity = context.ActList.Include(a => a.Coordinator).Include(b => b.Attendees).ThenInclude(c => c.SingleUser).FirstOrDefault(d => d.OccasionId == OccasionId);
                ViewBag.a = Activity;
                ViewBag.u = User;
                return View();
            }
        }

        [HttpGet("delete/{OccasionId}")]
        public IActionResult Delete(int OccasionId)
        {
            Occasion Activity = context.ActList.FirstOrDefault(a => a.OccasionId == OccasionId);
            context.ActList.Remove(Activity);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("join/{OccasionId}/{UserId}")]
        public IActionResult Attend(int OccasionId, int UserId)
        {
            Join NewJoin = new Join();
            NewJoin.UserId = UserId;
            NewJoin.OccasionId = OccasionId;
            context.Joinee.Add(NewJoin);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{OccasionId}/{UserId}")]
        public IActionResult Leave(int OccasionId, int UserId)
        {
            Join Leave = context.Joinee.FirstOrDefault(a => a.OccasionId == OccasionId && a.UserId == UserId);
            context.Joinee.Remove(Leave);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
