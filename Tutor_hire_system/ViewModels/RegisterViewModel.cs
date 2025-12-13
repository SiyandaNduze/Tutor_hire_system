using Tutor_hire_system.Models;

namespace Tutor_hire_system.ViewModels
{
    public class RegisterViewModel
    {
        public User User { get; set; } = new();
        public Student? Student { get; set; }
        public Tutor? Tutor { get; set; }
    }
}
