using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor_hire_system.Models
{
    public class Tutor
    {
        [Key]
        public int TutorId { get; set; }
        public int Age { get; set; }
        public string? Qualification { get; set; }
        public string? Subject { get; set; }
        public string? Location { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Job>? Jobs { get; set; }
    }
}
