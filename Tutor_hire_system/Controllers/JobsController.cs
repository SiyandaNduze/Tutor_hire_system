using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;
using Tutor_hire_system.Models;
using Tutor_hire_system.Services;

namespace Tutor_hire_system.Controllers
{
    public class JobsController : Controller
    {
        private readonly Tutor_hire_systemContext _context;
        private readonly NotificationService _notify;
        public JobsController(Tutor_hire_systemContext context, NotificationService notify)
        {
            _context = context;
            _notify = notify;
        }

        // GET: Jobs
        //public async Task<IActionResult> Index()
        //{
        //    var tutor_hire_systemContext = _context.Job.Include(j => j.Post).Include(j => j.Tutor);
        //    return View(await tutor_hire_systemContext.ToListAsync());
        //}

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Post)
                .Include(j => j.Tutor)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "Content");
            ViewData["TutorId"] = new SelectList(_context.Tutor, "TutorId", "TutorId");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,DateAccepted,PostId,TutorId")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "Content", job.PostId);
            ViewData["TutorId"] = new SelectList(_context.Tutor, "TutorId", "TutorId", job.TutorId);
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "Content", job.PostId);
            ViewData["TutorId"] = new SelectList(_context.Tutor, "TutorId", "TutorId", job.TutorId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,DateAccepted,PostId,TutorId")] Job job)
        {
            if (id != job.JobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.JobId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "Content", job.PostId);
            ViewData["TutorId"] = new SelectList(_context.Tutor, "TutorId", "TutorId", job.TutorId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Post)
                .Include(j => j.Tutor)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job != null)
            {
                _context.Job.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.JobId == id);
        }

        public async Task<IActionResult> Available()
        {
            var takenPostIds = await _context.Job
                .Where(j => j.Status != "Dropped")
                .Select(j => j.PostId)
                .ToListAsync();

            var posts = await _context.Post
                .Include(p => p.Student)
                .Where(p => !takenPostIds.Contains(p.PostId))
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            return View(posts);
        }

        // ---------------------------------------------------
        // ACCEPT A JOB
        // ---------------------------------------------------
        public async Task<IActionResult> Accept(int postId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var tutor = await _context.Tutor
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tutor == null) return Unauthorized();

            var post = await _context.Post
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null || post.Student == null)
                return BadRequest();

            var job = new Job
            {
                PostId = postId,
                TutorId = tutor.TutorId,
                DateAccepted = DateTime.Now,
                Status = "Accepted"
            };

            _context.Job.Add(job);
            await _context.SaveChangesAsync();

            // SEND STUDENT NOTIFICATION
            await _notify.SendNotification(
                post.Student.UserId,
                "A tutor accepted your post.",
                "Accepted",
                tutor.FullName,
                post.Content
            );

            return RedirectToAction("Index");
        }

        // ---------------------------------------------------
        // VIEW MY JOBS
        // ---------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var tutor = await _context.Tutor.FirstOrDefaultAsync(t => t.UserId == userId);
            if (tutor == null)
                return Unauthorized();

            var jobs = await _context.Job
                .Include(j => j.Post)
                .ThenInclude(p => p.Student)
                .Where(j => j.TutorId == tutor.TutorId)
                .OrderByDescending(j => j.DateAccepted)
                .ToListAsync();

            return View(jobs);
        }

        // ---------------------------------------------------
        // COMPLETE A JOB
        // ---------------------------------------------------
        public async Task<IActionResult> Complete(int id)
        {
            var job = await _context.Job
                .Include(j => j.Post)
                .ThenInclude(p => p.Student)
                .ThenInclude(s => s.User)
                .Include(j => j.Tutor)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null) return NotFound();

            job.Status = "Completed";
            await _context.SaveChangesAsync();

            await _notify.SendNotification(
                job.Post.Student.UserId,
                "Your job has been completed.",
                "Completed",
                job.Tutor.FullName,
                job.Post.Content
            );

            return RedirectToAction("Index");
        }


        // ---------------------------------------------------
        // DROP A JOB
        // ---------------------------------------------------
        public async Task<IActionResult> Drop(int id)
        {
            var job = await _context.Job
                .Include(j => j.Post)
                .ThenInclude(p => p.Student)
                .ThenInclude(s => s.User)
                .Include(j => j.Tutor)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null) return NotFound();

            job.Status = "Dropped";
            await _context.SaveChangesAsync();

            await _notify.SendNotification(
                job.Post.Student.UserId,
                "The tutor dropped your job.",
                "Dropped",
                job.Tutor.FullName,
                job.Post.Content
            );

            return RedirectToAction("Index");
        }
    }
}
