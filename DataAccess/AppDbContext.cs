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
        public DbSet<Teacher> Teachers { get; set; } = null!;  // NEW: Teacher DbSet
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Score> Scores { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<AcademicAnalysis> AcademicAnalyses { get; set; } = null!;
        public DbSet<LearningPathRecommendation> LearningPathRecommendations { get; set; } = null!;

        // New DbSets for AI Knowledge Base and Dashboard
        public DbSet<AIKnowledgeBase> AIKnowledgeBases { get; set; } = null!;
        public DbSet<AIConversationLog> AIConversationLogs { get; set; } = null!;
        public DbSet<DashboardMetric> DashboardMetrics { get; set; } = null!;

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

                entity.HasOne(e => e.Teacher)
                    .WithMany(t => t.Classes)
                    .HasForeignKey(e => e.TeacherId);
            });

            // Cấu hình Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeacherCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.TeacherCode).IsUnique();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Teacher>(e => e.UserId);
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

                entity.HasOne(e => e.Class)
                    .WithMany()
                    .HasForeignKey(e => e.ClassId)
                    .IsRequired(false)  // ClassId is optional
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .IsRequired(false)  // CourseId is optional
                    .OnDelete(DeleteBehavior.Restrict);
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

            // Cấu hình AIKnowledgeBase
            modelBuilder.Entity<AIKnowledgeBase>(entity =>
            {
                entity.HasKey(e => e.KnowledgeId);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.SubCategory).HasMaxLength(100);
                entity.Property(e => e.Tags).HasMaxLength(500);
                entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("vi");
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Priority);
            });

            // Cấu hình AIConversationLog
            modelBuilder.Entity<AIConversationLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.RequestType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Prompt).IsRequired();
                entity.Property(e => e.ModelUsed).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.RequestType);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Cấu hình DashboardMetric
            modelBuilder.Entity<DashboardMetric>(entity =>
            {
                entity.HasKey(e => e.MetricId);
                entity.Property(e => e.MetricName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Unit).HasMaxLength(20);
                entity.Property(e => e.Trend).HasMaxLength(20);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.IconClass).HasMaxLength(50);
                entity.Property(e => e.ColorClass).HasMaxLength(20);

                entity.HasIndex(e => e.MetricName).IsUnique();
                entity.HasIndex(e => e.Category);
            });
        }
    }
}
