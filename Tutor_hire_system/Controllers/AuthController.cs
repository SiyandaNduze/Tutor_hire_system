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

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            return View();
        }

        public ActionResult Logout()
        {
            return RedirectToAction();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(User user)
        {
            return View();
        }
    }
}
