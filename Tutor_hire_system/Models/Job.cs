using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor_hire_system.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Accepted")]
        public DateTime DateAccepted { get; set; } = DateTime.UtcNow;

        public string? Status { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post? Post { get; set; }

        [ForeignKey("Tutor")]
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }
    }
}
