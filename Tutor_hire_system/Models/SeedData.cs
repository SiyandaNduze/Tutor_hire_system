using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Data;

namespace Tutor_hire_system.Models
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new Tutor_hire_systemContext(
                serviceProvider.GetRequiredService<DbContextOptions<Tutor_hire_systemContext>>());

            // Roles
            if (!context.Role.Any())
            {
                context.Role.AddRange(
                    new Role { RoleName = "Tutor", Description = "Views posts and accepts jobs"},
                    new Role { RoleName = "Student", Description = "Upload posts"},
                    new Role { RoleName = "Admin", Description = "Maintains user data"}
                    );
                context.SaveChanges();
            }

            // Get role references
            var tutorRole = context.Role.First(r => r.RoleName == "Tutor");
            var studentRole = context.Role.First(r => r.RoleName == "Student");
            var adminRole = context.Role.First(r => r.RoleName == "admin");

            // Users
            if (!context.User.Any())
            {
                context.User.AddRange(
                    new User { UserId = 1, FirstName = "John", Surname = "Doe", Email = "jDoe@gmail.com", PhoneNumber = "0782347654", Password = "jdPass123", RoleId = tutorRole.RoleId },
                    new User { UserId = 2, FirstName = "Sam", Surname = "Smith", Email = "Smith@gmail.com", PhoneNumber = "0662456357", Password = "ssPass123", RoleId = studentRole.RoleId },
                    new User { UserId = 3, FirstName = "Jane", Surname = "Williams", Email = "Will@gmail.com", PhoneNumber = "0719872563", Password = "jwPass123", RoleId = adminRole.RoleId }
                    );
            }
            context.SaveChanges();

            // Students
            if (!context.Student.Any())
            {
                context.Student.AddRange(
                    new Student { StudentId = 1, StudentNumber = "123456789", Age = 21, Campus = "Mv Campus", UserId = 2 }
                    );
            }
            context.SaveChanges();

            // Tutors
            if ( context.Tutor.Any())
            {
                context.Tutor.AddRange(
                    new Tutor { TutorId = 1, Age = 37, Qualification = "Masters", Subject = "Math", Location = "Durban", UserId = 1 }
                    );
            }
            context.SaveChanges();
        }
    }
}
