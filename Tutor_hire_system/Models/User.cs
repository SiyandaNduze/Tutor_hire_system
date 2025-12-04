using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Tutor_hire_system.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Password { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public Student? Student { get; set; }
        public Tutor? Tutor { get; set; }
    }
}
