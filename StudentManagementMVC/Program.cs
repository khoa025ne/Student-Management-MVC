using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Services.Models;
using StudentManagementMVC.Middlewares;
using StudentManagementMVC.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 📡 SignalR for real-time notifications
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

// 7. Email & AI Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGeminiAIService, GeminiAIService>();

// 8. Background Services
builder.Services.AddHostedService<StudentManagementMVC.Services.AcademicWarningBackgroundService>();

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
