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
            ViewBag.Roles = new SelectList(_context.Role.ToList(), "RoleId", "RoleName");
            return View(new RegisterViewModel());
        }

        // ===== REGISTER ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) 
                return View(vm);

            // Check if email already exists
            if (_context.User.Any(u => u.Email == vm.User.Email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View(vm);
            }

            // Create a new user record
            var newUser = new User
            {
                FirstName = vm.User.FirstName,
                Surname = vm.User.Surname,
                Email = vm.User.Email,
                PhoneNumber = vm.User.PhoneNumber,
                RoleId = vm.User.RoleId,
            };

            // Hash the password
            newUser.Password = _passwordHasher.HashPassword(newUser, vm.User.Password ?? string.Empty);

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            // reload saved user including role
            var savedUser = await _context.User.Include(u => u.Role)
                                               .FirstOrDefaultAsync(u => u.UserId == newUser.UserId);

            // link extra info based on role
            var roleName = savedUser?.Role?.RoleName;

            if (roleName == "Student")
            {
                vm.Student.UserId = savedUser.UserId;
                _context.Student.Add(vm.Student);
                await _context.SaveChangesAsync();
            }
            else if (roleName == "Tutor")
            {
                vm.Tutor.UserId = savedUser.UserId;
                _context.Tutor.Add(vm.Tutor);
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.SetInt32("UserId", savedUser.UserId);
            HttpContext.Session.SetString("Role", roleName ?? string.Empty);

            // Redirect based on role
            return roleName switch
            {
                "Student" => RedirectToAction("", ""),
                "Tutor" => RedirectToAction("", ""),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
