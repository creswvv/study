using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Data;

public class CollegeDbContext : DbContext
{
    public CollegeDbContext(DbContextOptions<CollegeDbContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<AttendanceRecord> Attendance => Set<AttendanceRecord>();
    public DbSet<UserAccount> Accounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Person -> Student / Teacher (одна таблица People)
        mb.Entity<Person>();
        mb.Entity<Teacher>();
        mb.Entity<Student>(b =>
        {
            b.HasIndex(s => s.StudentId).IsUnique();
            b.HasOne(s => s.Group).WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId).OnDelete(DeleteBehavior.SetNull);
            b.HasMany(s => s.Grades).WithOne(g => g.Student)
                .HasForeignKey(g => g.PersonId).OnDelete(DeleteBehavior.Cascade);
            b.Navigation(s => s.Grades).HasField("_grades");
        });

        mb.Entity<Group>(b => b.Navigation(g => g.Students).HasField("_students"));

        mb.Entity<Subject>(b =>
            b.HasOne(s => s.Teacher).WithMany().HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.SetNull));

        mb.Entity<Grade>(b =>
            b.HasOne(g => g.Subject).WithMany().HasForeignKey(g => g.SubjectId).OnDelete(DeleteBehavior.Cascade));

        mb.Entity<AttendanceRecord>(b =>
        {
            b.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.PersonId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(a => new { a.PersonId, a.Date }).IsUnique();
        });

        // UserAccount -> User -> AdminUser / TeacherUser / StudentUser
        mb.Entity<UserAccount>(b =>
        {
            b.HasIndex(u => u.LoginName).IsUnique();
            b.Property<string>("PasswordHash");          // private-свойство отображается через строковое имя
        });
        mb.Entity<User>();
        mb.Entity<AdminUser>();
        mb.Entity<TeacherUser>();
        mb.Entity<StudentUser>();
    }
}
