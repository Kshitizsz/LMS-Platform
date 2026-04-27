using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Progress> Progresses => Set<Progress>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // User
        mb.Entity<User>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).IsRequired().HasMaxLength(256);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Role).HasMaxLength(20);
        });

        // Course
        mb.Entity<Course>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Instructor)
             .WithMany(x => x.CreatedCourses)
             .HasForeignKey(x => x.InstructorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Enrollment (unique per user+course)
        mb.Entity<Progress>(e => {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.EnrollmentId, x.LessonId }).IsUnique();

            // Restrict cascade to avoid multiple cascade path error in SQL Server
            e.HasOne(x => x.Enrollment)
             .WithMany(x => x.Progresses)
             .HasForeignKey(x => x.EnrollmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Lesson)
             .WithMany(x => x.Progresses)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}