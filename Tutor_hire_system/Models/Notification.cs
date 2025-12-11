using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor_hire_system.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }  // Accepted / Completed / Dropped

        public string? TutorName { get; set; }  // Name of tutor sending update
        public string? PostContent { get; set; } // Related post content
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}
