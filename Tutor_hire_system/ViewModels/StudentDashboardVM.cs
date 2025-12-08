using Tutor_hire_system.Models;

namespace Tutor_hire_system.ViewModels
{

    public class StudentDashboardVM
    {
        public List<Post>? OtherStudentsPosts { get; set; }
        public int TotalMyPosts { get; set; }
    }
}
