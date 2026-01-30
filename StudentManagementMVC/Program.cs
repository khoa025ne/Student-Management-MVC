using DataAccess;
using DataAccess.Repositories.Implementations;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Services.Implementations;
using Services.Interfaces;
using Services.Models;
using StudentManagementMVC.Middlewares;
using StudentManagementMVC.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure UTF-8 encoding globally
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddControllersWithViews();

// �️ Session Configuration for storing user data
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// �📡 SignalR for real-time notifications
builder.Services.AddSignalR();

// 1. Cấu hình DbContext (Kết nối MySQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // Phiên bản MySQL
    ));

// 2. Cấu hình Authentication (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
    });

// 3. Cấu hình Email & Gemini AI từ appsettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiAI"));

// 4. HttpClient cho Gemini AI
builder.Services.AddHttpClient<IGeminiAIService, GeminiAIService>();

// 5. Đăng ký Repository (DI)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILearningPathRepository, LearningPathRepository>();
builder.Services.AddScoped<IAcademicAnalysisRepository, AcademicAnalysisRepository>();

// 6. Đăng ký Service (DI)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IScoreService, EnhancedScoreService>(); // ✅ Enhanced với AI
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILearningPathService, LearningPathService>();
builder.Services.AddScoped<IAcademicAnalysisService, AcademicAnalysisService>(); // ✅ Academic Analysis Service
builder.Services.AddScoped<IDashboardService, DashboardService>(); // ✅ Dashboard Service
builder.Services.AddScoped<IAIKnowledgeBaseService, AIKnowledgeBaseService>(); // ✅ AI Knowledge Base Service
builder.Services.AddScoped<ITeacherService, TeacherService>(); // ✅ Teacher Service - 3-Layer Architecture
builder.Services.AddScoped<IAcademicWarningService, AcademicWarningService>(); // ✅ Academic Warning Service

// 7. Email & AI Services
builder.Services.AddScoped<IEmailService, EmailService>();
// Note: IGeminiAIService already registered via AddHttpClient above

// 8. Background Services (hosts in Presentation layer, logic in Services layer)
builder.Services.AddHostedService<StudentManagementMVC.BackgroundServices.AcademicWarningBackgroundService>();

var app = builder.Build();

// 🔧 Fix Foreign Key Constraint cho Notifications (chỉ chạy 1 lần)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE notifications DROP FOREIGN KEY FK_Notifications_Students_StudentId;
            ALTER TABLE notifications ADD CONSTRAINT FK_Notifications_Students_StudentId 
            FOREIGN KEY (StudentId) REFERENCES students(StudentId) ON DELETE CASCADE;
        ");
        Console.WriteLine("✅ Updated FK constraint: notifications.StudentId -> CASCADE DELETE");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ FK constraint already updated or error: {ex.Message}");
    }
    
    // Add missing columns to notifications table (MySQL compatible)
    var columnsToAdd = new Dictionary<string, string>
    {
        ["CreatedBy"] = "VARCHAR(255) NULL",
        ["Priority"] = "VARCHAR(20) DEFAULT 'medium'",
        ["Link"] = "VARCHAR(200) NULL",
        ["ReadAt"] = "DATETIME NULL",
        ["TeacherId"] = "INT NULL"
    };
    
    foreach (var column in columnsToAdd)
    {
        try
        {
            // Check if column exists
            var checkSql = $@"SELECT COUNT(*) FROM information_schema.columns 
                              WHERE table_schema = DATABASE() 
                              AND table_name = 'notifications' 
                              AND column_name = '{column.Key}'";
            var exists = await context.Database.ExecuteSqlRawAsync(checkSql);
            
            // Try to add column (will fail if exists, which is fine)
            var addSql = $"ALTER TABLE notifications ADD COLUMN {column.Key} {column.Value}";
            await context.Database.ExecuteSqlRawAsync(addSql);
            Console.WriteLine($"✅ Added column {column.Key} to notifications table");
        }
        catch (Exception)
        {
            // Column already exists, ignore
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // 🗃️ Enable session

app.UseAuthentication(); // Bật xác thực
app.UseAuthorization();  // Bật phân quyền

// Middleware buộc sinh viên chọn ngành
app.UseMiddleware<RequireMajorMiddleware>();

// 📡 Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
