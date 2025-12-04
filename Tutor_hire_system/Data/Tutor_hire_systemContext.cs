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
    }
}
