using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;
using Tutor_hire_system.Models;
using Tutor_hire_system.ViewModels;

namespace Tutor_hire_system.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly Tutor_hire_systemContext _context;

        public AnalyticsController(Tutor_hire_systemContext context)
        {
            _context = context;
        }

        // ================= ADMIN ANALYTICS =================
        public async Task<IActionResult> Admin()
        {
            var vm = new AdminAnalyticsViewModel
            {
                // USERS
                TotalUsers = await _context.User.CountAsync(),
                TotalStudents = await _context.Student.CountAsync(),
                TotalTutors = await _context.Tutor.CountAsync(),

                // POSTS
                TotalPosts = await _context.Post.CountAsync(),
                PendingPosts = await _context.Post
                    .Where(p => p.IsAccepted == "Pending")
                    .CountAsync(),

                // JOBS
                JobsAccepted = await _context.Job
                    .Where(j => j.Status == "Accepted")
                    .CountAsync(),

                JobsCompleted = await _context.Job
                    .Where(j => j.Status == "Completed")
                    .CountAsync(),

                JobsDropped = await _context.Job
                    .Where(j => j.Status == "Dropped")
                    .CountAsync()
            };

            return View(vm);
        }

        // ================= USERS =================
        public async Task<IActionResult> Users()
        {
            var users = await _context.User
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Students()
        {
            var students = await _context.Student
                .Include(s => s.User)
                .ToListAsync();

            return View(students);
        }

        public async Task<IActionResult> Tutors()
        {
            var tutors = await _context.Tutor
                .Include(t => t.User)
                .ToListAsync();

            return View(tutors);
        }

        // ================= POSTS =================
        public async Task<IActionResult> Posts()
        {
            var posts = await _context.Post
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> PendingPosts()
        {
            var posts = await _context.Post
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .Where(p => p.IsAccepted == "Pending")
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            return View("Posts", posts);
        }

        // ================= JOBS =================
        public async Task<IActionResult> JobsAccepted()
        {
            return View("Jobs", await GetJobsByStatus("Accepted"));
        }

        public async Task<IActionResult> JobsCompleted()
        {
            return View("Jobs", await GetJobsByStatus("Completed"));
        }

        public async Task<IActionResult> JobsDropped()
        {
            return View("Jobs", await GetJobsByStatus("Dropped"));
        }

        private async Task<List<Job>> GetJobsByStatus(string status)
        {
            return await _context.Job
                .Include(j => j.Post)
                .ThenInclude(p => p.Student)
                .ThenInclude(s => s.User)
                .Include(j => j.Tutor)
                .ThenInclude(t => t.User)
                .Where(j => j.Status == status)
                .OrderByDescending(j => j.DateAccepted)
                .ToListAsync();
        }
    }
}
