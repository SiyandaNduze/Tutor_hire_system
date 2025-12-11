using Tutor_hire_system.Data;
using Tutor_hire_system.Models;

namespace Tutor_hire_system.Services
{
    public class NotificationService
    {
        private readonly Tutor_hire_systemContext _context;

        public NotificationService(Tutor_hire_systemContext context)
        {
            _context = context;
        }

        public async Task SendNotification(
            int userId,
            string message,
            string status,
            string tutorName,
            string postContent)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Status = status,
                TutorName = tutorName,
                PostContent = postContent,
                CreatedAt = DateTime.Now
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
