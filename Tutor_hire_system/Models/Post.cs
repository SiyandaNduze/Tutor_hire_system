using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor_hire_system.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public string? Content { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        [Display(Name = "Is Accepted")]
        public string? IsAccepted { get; set; } = "Pending";

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public Job? Job { get; set; }
    }
}
