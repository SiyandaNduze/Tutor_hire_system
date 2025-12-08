using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;
using Tutor_hire_system.Models;
using Tutor_hire_system.ViewModels;

namespace Tutor_hire_system.Controllers
{
    public class DashboardController : Controller
    {
        private readonly Tutor_hire_systemContext _context;

        public DashboardController(Tutor_hire_systemContext context)
        {
            _context = context;
        }

        // -------------------------------
        // TUTOR DASHBOARD
        // -------------------------------
        public async Task<IActionResult> Tutor()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var tutor = await _context.Tutor.FirstOrDefaultAsync(t => t.UserId == userId);

            if (tutor == null) return Unauthorized(); // user not a tutor

            var jobs = await _context.Job
                .Where(j => j.TutorId == tutor.TutorId)
                .ToListAsync();

            var vm = new TutorDashboardVM
            {
                JobsTaken = jobs.Count,
                JobsCompleted = jobs.Count(j => j.Status == "Completed"),
                JobsPending = jobs.Count(j => j.Status == "Pending"),
                RecentJobs = jobs.OrderByDescending(j => j.DateAccepted).Take(5).ToList()
            };

            return View(vm);
        }

        // -------------------------------
        // STUDENT DASHBOARD
        // -------------------------------
        public async Task<IActionResult> Student()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var student = await _context.Student.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Unauthorized();

            var vm = new StudentDashboardVM
            {
                OtherStudentsPosts = await _context.Post
                    .Include(p => p.Student)
                    .Where(p => p.StudentId != student.StudentId)
                    .OrderByDescending(p => p.DateCreated)
                    .ToListAsync(),

                TotalMyPosts = await _context.Post.CountAsync(p => p.StudentId == student.StudentId)
            };

            return View(vm);
        }

        // -------------------------------
        // ADMIN DASHBOARD
        // -------------------------------
        public async Task<IActionResult> Admin()
        {
            var vm = new AdminDashboardVM
            {
                TotalUsers = await _context.User.CountAsync(),
                TotalStudents = await _context.Student.CountAsync(),
                TotalTutors = await _context.Tutor.CountAsync(),
                TotalPosts = await _context.Post.CountAsync(),

                JobsAccepted = await _context.Job.CountAsync(j => j.Status == "Accepted"),
                JobsCompleted = await _context.Job.CountAsync(j => j.Status == "Completed"),
                JobsDropped = await _context.Job.CountAsync(j => j.Status == "Dropped")
            };

            return View(vm);
        }
    }
}
