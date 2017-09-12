using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;

namespace WebOppointmentApi.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Organazition> Organazitions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Scheduling> Schedulings { get; set; }
        public DbSet<Source> Source { get; set; }
        public DbSet<Registered> Registereds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hospital>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<Organazition>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<User>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<Permission>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<Role>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<Scheduling>().HasQueryFilter(m => m.Status != "删除");
            modelBuilder.Entity<Registered>().HasQueryFilter(m => m.Status != "删除");
        }
    }
}
