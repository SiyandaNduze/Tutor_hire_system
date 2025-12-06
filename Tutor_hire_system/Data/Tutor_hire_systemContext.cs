using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tutor_hire_system.Models;

namespace Tutor_hire_system.Data
{
    public class Tutor_hire_systemContext : DbContext
    {
        public Tutor_hire_systemContext (DbContextOptions<Tutor_hire_systemContext> options)
            : base(options)
        {
        }

        public DbSet<Tutor_hire_system.Models.User> User { get; set; } = default!;
        public DbSet<Tutor_hire_system.Models.Student> Student { get; set; } = default!;
        public DbSet<Tutor_hire_system.Models.Tutor> Tutor { get; set; } = default!;
        public DbSet<Tutor_hire_system.Models.Post> Post { get; set; } = default!;
        public DbSet<Tutor_hire_system.Models.Job> Job { get; set; } = default!;
        public DbSet<Tutor_hire_system.Models.Role> Role { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ROLE 1-M USER
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // USER 1-1 STUDENT
            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER 1-1 TUTOR
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tutor)
                .WithOne(t => t.User)
                .HasForeignKey<Tutor>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // STUDENT 1-M POST
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Posts)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // POST 0-1 JOB
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Job)
                .WithOne(j => j.Post)
                .HasForeignKey<Job>(j => j.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // TUTOR 1-M JOB
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Tutor)
                .WithMany(t => t.Jobs)
                .HasForeignKey(j => j.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE CONSTRAINT: A Post can only have ONE job
            modelBuilder.Entity<Job>()
                .HasIndex(j => j.PostId)
                .IsUnique();
        }
    }
}
