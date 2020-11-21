using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Infrastructure
{
    public class CourseEnrollmentContext : DbContext
    {
        public CourseEnrollmentContext(DbContextOptions options)
         : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                 .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Course>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Name).IsUnique();
        }
    }

    public class CourseEnrollmentContextFactory : IDesignTimeDbContextFactory<CourseEnrollmentContext>
    {
        public CourseEnrollmentContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CourseEnrollmentContext>();
            optionsBuilder.UseSqlite("Data Source=courseenrollment.db");

            return new CourseEnrollmentContext(optionsBuilder.Options);
        }
    }

}