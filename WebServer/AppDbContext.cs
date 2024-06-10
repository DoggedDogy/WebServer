using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Prof> Profs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<UserRoleClaim> UserRoleClaims { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Labwork> Labworks { get; set; }
    }
}
