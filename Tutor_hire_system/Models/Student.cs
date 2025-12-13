using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor_hire_system.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        [Required]
        [Display(Name = "Student Number")]
        public string? StudentNumber { get; set; }
        public int Age { get; set; }
        public string? Campus { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Post>? Posts { get; set; }
        [NotMapped]
        public string FullName => $"{User?.FirstName} {User?.Surname}";
    }
}
