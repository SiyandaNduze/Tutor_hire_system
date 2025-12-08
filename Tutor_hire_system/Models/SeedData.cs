using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
                    new Role { RoleName = "Tutor", Description = "Views posts and accepts jobs" },
                    new Role { RoleName = "Student", Description = "Upload posts" },
                    new Role { RoleName = "Admin", Description = "Maintains user data" }
                );
                context.SaveChanges();
            }

            // get role references 
            var tutorRole = context.Role.First(r => r.RoleName == "Tutor");
            var studentRole = context.Role.First(r => r.RoleName == "Student");
            var adminRole = context.Role.First(r => r.RoleName == "Admin");

            var hasher = new PasswordHasher<User>();

            // Users
            if (!context.User.Any())
            {
                var john = new User
                {
                    FirstName = "John",
                    Surname = "Doe",
                    Email = "jDoe@gmail.com",
                    PhoneNumber = "0782347654",
                    RoleId = tutorRole.RoleId
                };
                john.Password = hasher.HashPassword(john, "jdPass123");

                var sam = new User
                {
                    FirstName = "Sam",
                    Surname = "Smith",
                    Email = "Smith@gmail.com",
                    PhoneNumber = "0662456357",
                    RoleId = studentRole.RoleId
                };
                sam.Password = hasher.HashPassword(sam, "ssPass123");

                var jane = new User
                {
                    FirstName = "Jane",
                    Surname = "Williams",
                    Email = "Will@gmail.com",
                    PhoneNumber = "0719872563",
                    RoleId = adminRole.RoleId
                };
                jane.Password = hasher.HashPassword(jane, "jwPass123");

                context.User.AddRange(john, sam, jane);
                context.SaveChanges();
            }

            // Students (link to user by email to avoid relying on hard-coded IDs)
            if (!context.Student.Any())
            {
                var studentUser = context.User.FirstOrDefault(u => u.Email == "Smith@gmail.com");
                if (studentUser != null)
                {
                    context.Student.Add(
                        new Student
                        {
                            StudentNumber = "123456789",
                            Age = 21,
                            Campus = "Mv Campus",
                            UserId = studentUser.UserId
                        });
                    context.SaveChanges();
                }
            }

            // Tutors
            if (!context.Tutor.Any())
            {
                var tutorUser = context.User.FirstOrDefault(u => u.Email == "jDoe@gmail.com");
                if (tutorUser != null)
                {
                    context.Tutor.Add(
                        new Tutor
                        {
                            Age = 37,
                            Qualification = "Masters", // ensure your Tutor model has these properties
                            Subject = "Math",
                            Location = "Durban",
                            UserId = tutorUser.UserId
                        });
                    context.SaveChanges();
                }
            }
        }
    }
}
