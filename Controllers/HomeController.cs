using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bankaccounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace bankaccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankAccountsContext db;

        public HomeController(BankAccountsContext context)
        {
            db = context;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");

                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                db.Add(newUser);
                db.SaveChanges();

                HttpContext.Session.SetInt32("userId", newUser.UserId);
                return RedirectToAction("Account");
            }

            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.FirstOrDefault(user => user.Email == userSubmission.Email);

                if (user == null)
                {
                    ModelState.AddModelError("Email", "Invalid email or password.");
                    return View("Login");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, user.Password, userSubmission.Password);

                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid email or password");
                    return View("Login");
                }

                HttpContext.Session.SetInt32("userId", user.UserId);
                return RedirectToAction("Account");
            }

            return View("Login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("account")]
        public IActionResult Account()
        {
            int? userId = HttpContext.Session.GetInt32("userId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            User currentUser = db.Users.FirstOrDefault(user => user.UserId == (int)userId);
            List<Transaction> transactions = db.Transactions
                .Where(t => t.UserId == (int)userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();
            
                
            ViewBag.CurrentUser = currentUser;
            ViewBag.Transactions = transactions;
            decimal? total = db.Transactions
                .Where(t => t.UserId == (int)userId)
                .Sum(t => t.Amount);
            if (total == null)
            {
                total = 0;
            }
            ViewBag.Total = (decimal)total;
            return View("Account");
        }

        [HttpPost("create/transaction")]
        public IActionResult Create(Transaction trans)
        {
            if (ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                trans.UserId = (int)userId;
                db.Add(trans);
                db.SaveChanges();
                return RedirectToAction("Account");
            }
            return View("Account");
        }
    }
}
