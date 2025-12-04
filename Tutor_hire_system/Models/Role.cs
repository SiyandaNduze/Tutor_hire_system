using System.ComponentModel.DataAnnotations;

namespace Tutor_hire_system.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
        [Required]
        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
