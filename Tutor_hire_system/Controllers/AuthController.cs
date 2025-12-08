using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;
using Tutor_hire_system.Models;

namespace Tutor_hire_system.Controllers
{
    public class AuthController : Controller
    {
       private readonly Tutor_hire_systemContext _context;
        public AuthController(Tutor_hire_systemContext context)
        {
            _context = context;
        }

        // GET: Auth
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // ===== LOGIN =====
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            // Validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Erroe = "Email and password are required";
                return View();
            }

            // Query the database for the user, including their role
            var user = _context.User.Include(x => x.Role).FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password!";
                return View();
            }

            // Store user info in session for later use (e.g., in other controllers)
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role?.RoleName ?? "");

            // Redirect based on role
            return user.Role?.RoleName switch
            {
                "Admin" => RedirectToAction("", ""),
                "Student" => RedirectToAction("", ""),
                "Tutor" => RedirectToAction("",""),
                _ => RedirectToAction("Index","Home")
            };
        }

        // ===== LOGOUT =====
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }

        
        public ActionResult Register()
        {
            return View();
        }

        // ===== REGISTER ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(User user, Student student, Tutor tutor)
        {
            if (!ModelState.IsValid) 
                return View(user);

            // Check if email already exists
            if (_context.User.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View(user);
            }

            // Create a new user record
            var newUser = new User
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                Role = user.Role,
            };
            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            // Link user to student or lecturer
            if (user.Role?.RoleName == "Student")
            {
                student.UserId = newUser.UserId;
                _context.Student.Add(student);
                await _context.SaveChangesAsync();
            }
            else if (user.Role?.RoleName == "Tutor")
            {
                tutor.UserId = newUser.UserId;
                _context.Tutor.Add(tutor);
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role?.RoleName ?? "");

            // Redirect based on role
            return user.Role?.RoleName switch
            {
                "Student" => RedirectToAction("", ""),
                "Tutor" => RedirectToAction("", ""),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
