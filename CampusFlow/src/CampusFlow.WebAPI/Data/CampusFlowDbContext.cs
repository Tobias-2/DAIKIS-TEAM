using CampusFlow.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusFlow.WebAPI.Data{

public class CampusFlowDbContext(DbContextOptions<CampusFlowDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>().HasIndex(c => c.Code).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(s => s.Email).IsUnique();
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();
    }
}
}