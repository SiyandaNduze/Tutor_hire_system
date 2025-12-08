using Tutor_hire_system.Models;

namespace Tutor_hire_system.ViewModels
{
    public class TutorDashboardVM
    {
        public int JobsTaken { get; set; }
        public int JobsCompleted { get; set; }
        public int JobsPending { get; set; }
        public List<Job>? RecentJobs { get; set; }
    }
}
