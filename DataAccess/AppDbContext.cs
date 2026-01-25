using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    /// <summary>
    /// DbContext chính cho ứng dụng Student Management
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Score> Scores { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<AcademicAnalysis> AcademicAnalyses { get; set; } = null!;
        public DbSet<LearningPathRecommendation> LearningPathRecommendations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
            });

            // Cấu hình User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId);
            });

            // Cấu hình Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.StudentCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                
                // Cấu hình Major enum lưu dưới dạng int
                entity.Property(e => e.Major)
                    .HasConversion<int>();

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Student>(e => e.UserId);
            });

            // Cấu hình Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.CourseCode).IsUnique();
                entity.Property(e => e.CourseName).HasMaxLength(200).IsRequired();

                entity.HasOne(e => e.PrerequisiteCourse)
                    .WithMany()
                    .HasForeignKey(e => e.PrerequisiteCourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình Semester
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId);
                entity.Property(e => e.SemesterCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.SemesterCode).IsUnique();
                entity.Property(e => e.SemesterName).HasMaxLength(100).IsRequired();
            });

            // Cấu hình Class
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => e.ClassId);
                entity.Property(e => e.ClassCode).HasMaxLength(20).IsRequired();
                entity.Property(e => e.ClassName).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Classes)
                    .HasForeignKey(e => e.CourseId);

                entity.HasOne(e => e.Semester)
                    .WithMany(s => s.Classes)
                    .HasForeignKey(e => e.SemesterId);
            });

            // Cấu hình Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Class)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.ClassId);
            });

            // Cấu hình Score
            modelBuilder.Entity<Score>(entity =>
            {
                entity.HasKey(e => e.ScoreId);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Scores)
                    .HasForeignKey(e => e.StudentId);
            });

            // Cấu hình AcademicAnalysis
            modelBuilder.Entity<AcademicAnalysis>(entity =>
            {
                entity.HasKey(e => e.AnalysisId);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Analyses)
                    .HasForeignKey(e => e.StudentId);
            });

            // Cấu hình LearningPathRecommendation
            modelBuilder.Entity<LearningPathRecommendation>(entity =>
            {
                entity.HasKey(e => e.RecommendationId);

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Semester)
                    .WithMany()
                    .HasForeignKey(e => e.SemesterId);
            });

            // Cấu hình Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade); // Xóa cascade khi student bị xóa
            });
        }
    }
}
