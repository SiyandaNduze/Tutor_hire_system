using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;
using Tutor_hire_system.Models;
using Tutor_hire_system.ViewModels;

namespace Tutor_hire_system.Controllers
{
    public class AuthController : Controller
    {
       private readonly Tutor_hire_systemContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        public AuthController(Tutor_hire_systemContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: Auth
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ===== LOGIN =====
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required";
                return View();
            }

            // Query the database for the user, including their role
            var user = _context.User.Include(x => x.Role).FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password!";
                return View();
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.Password ?? string.Empty, password);
            if (verify == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password!";
                return View();
            }

            // Store user info in session for later use (e.g., in other controllers)
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role?.RoleName ?? string.Empty);

            // Redirect based on role
            return user.Role?.RoleName switch
            {
                "Admin" => RedirectToAction("Admin", "Dashboard"),
                "Student" => RedirectToAction("Student", "Dashboard"),
                "Tutor" => RedirectToAction("Tutor", "Dashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ===== LOGOUT =====
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }


        public IActionResult Register()
        {
            ViewBag.Roles = new SelectList(
                _context.Role
                    .Where(r => r.RoleName != "Admin"), // ✅ exclude admin
                "RoleId",
                "RoleName"
            );

            return View();
        }


        // ===== REGISTER ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            ViewBag.Roles = new SelectList(
                _context.Role.Where(r => r.RoleName != "Admin"),
                "RoleId",
                "RoleName"
            );

            if (vm.User.RoleId != null)
            {
                var role = await _context.Role.FindAsync(vm.User.RoleId);

                if (role?.RoleName == "Student" && vm.Student?.Age == null)
                {
                    ModelState.AddModelError("Student.Age", "Age is required");
                }

                if (role?.RoleName == "Tutor" && vm.Tutor?.Age == null)
                {
                    ModelState.AddModelError("Tutor.Age", "Age is required");
                }
            }

            if (!ModelState.IsValid)
                return View(vm);


            if (_context.User.Any(u => u.Email == vm.User.Email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View(vm);
            }

            var newUser = new User
            {
                FirstName = vm.User.FirstName,
                Surname = vm.User.Surname,
                Email = vm.User.Email,
                PhoneNumber = vm.User.PhoneNumber,
                RoleId = vm.User.RoleId
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, vm.User.Password!);

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            var roleName = (await _context.Role.FindAsync(newUser.RoleId))?.RoleName;

            if (roleName == "Student" && vm.Student != null)
            {
                vm.Student.UserId = newUser.UserId;
                _context.Student.Add(vm.Student);
            }
            else if (roleName == "Tutor" && vm.Tutor != null)
            {
                vm.Tutor.UserId = newUser.UserId;
                _context.Tutor.Add(vm.Tutor);
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("Role", roleName ?? "");

            return roleName switch
            {
                "Student" => RedirectToAction("Student", "Dashboard"),
                "Tutor" => RedirectToAction("Tutor", "Dashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
