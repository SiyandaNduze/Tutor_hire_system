using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;

public class NotificationController : Controller
{
    private readonly Tutor_hire_systemContext _context;

    public NotificationController(Tutor_hire_systemContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var notes = await _context.Notification
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(notes);
    }

    public async Task<IActionResult> MarkAsRead(int id)
    {
        var note = await _context.Notification.FindAsync(id);
        if (note != null)
        {
            note.IsRead = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
