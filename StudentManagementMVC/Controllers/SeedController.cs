using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
// SeedController is a special controller for seeding initial data
// It is acceptable to use Entities directly here as it's for development/setup purposes only
using DataAccess.Entities;


namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller tạo dữ liệu mẫu - chỉ dùng cho môi trường Development
    /// Tuân thủ nguyên tắc: Controller chỉ gọi Services, không gọi trực tiếp Repositories
    /// NOTE: Using DataAccess.Entities is acceptable here as this is seed data controller
    /// </summary>
    public class SeedController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IRoleService _roleService;
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;


        public SeedController(
            IUserService userService,
            IStudentService studentService,
            IRoleService roleService,
            IClassService classService,
            ICourseService courseService,
            ISemesterService semesterService)
        {
            _userService = userService;
            _studentService = studentService;
            _roleService = roleService;
            _classService = classService;
            _courseService = courseService;
            _semesterService = semesterService;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Seed Roles
            var roles = new List<Role>
            {
                new Role { RoleName = "Admin", Description = "Administrator with full access" },
                new Role { RoleName = "Manager", Description = "Staff manager" },
                new Role { RoleName = "Teacher", Description = "Lecturer/Teacher" },
                new Role { RoleName = "Student", Description = "University Student" }
            };

            foreach (var role in roles)
            {
                var allRoles = await _roleService.GetAllAsync();
                if (!allRoles.Any(r => r.RoleName == role.RoleName))
                {
                    await _roleService.CreateAsync(role);
                }
            }

            // Helper to Create User if not exists
            async Task CreateUserIfNotExists(string email, string fullName, string password, string roleName, string phoneNumber = "0000000000")
            {
                var existingUser = await _userService.GetByEmailAsync(email);
                if (existingUser == null)
                {
                    var role = await _roleService.GetByNameAsync(roleName);
                    if (role != null)
                    {
                        var newUser = new User
                        {
                            FullName = fullName,
                            Email = email,
                            PhoneNumber = phoneNumber,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            RoleId = role.RoleId,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        };
                        await _userService.CreateAsync(newUser);
                    }
                }
            }

            // 2. Seed Users for 4 Roles
            await CreateUserIfNotExists("admin@student.com", "System Administrator", "123123", "Admin", "0999999999");
            await CreateUserIfNotExists("manager@student.com", "Training Manager", "123123", "Manager", "0988888888");
            await CreateUserIfNotExists("teacher@student.com", "Nguyen Van Teacher", "123123", "Teacher", "0977777777");

            // 3. Seed Semesters
            var semesterData = new[] 
            { 
                new { Name = "Học kỳ 1 - 2024", Code = "FA24" }, 
                new { Name = "Học kỳ 2 - 2024", Code = "SP25" } 
            };

            foreach (var item in semesterData)
            {
                var allSemesters = await _semesterService.GetAllAsync();
                if (!allSemesters.Any(s => s.SemesterName == item.Name))
                {
                    await _semesterService.CreateAsync(new Semester 
                    { 
                        SemesterName = item.Name,
                        SemesterCode = item.Code,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(4),
                        IsActive = true
                    });
                }
            }

            // 4. Seed Courses
            var courses = new[] 
            { 
                new Course { CourseName = "Lập trình .NET", CourseCode = "PRN211", Credits = 3, Major = "CNTT" },
                new Course { CourseName = "Cấu trúc dữ liệu", CourseCode = "CSD201", Credits = 4, Major = "CNTT" },
                new Course { CourseName = "Cơ sở dữ liệu", CourseCode = "DBI202", Credits = 3, Major = "CNTT" }
            };
            foreach (var c in courses)
            {
                var allCourses = await _courseService.GetAllAsync();
                if (!allCourses.Any(x => x.CourseName == c.CourseName))
                {
                    await _courseService.CreateAsync(c);
                }
            }

            // 5. Seed Classes
            var firstSemester = (await _semesterService.GetAllAsync()).FirstOrDefault();
            var firstCourse = (await _courseService.GetAllAsync()).FirstOrDefault();

            if (firstSemester != null && firstCourse != null)
            {
                var classNames = new[] { "SE1801", "SE1802", "SE1803" };
                foreach (var name in classNames)
                {
                    var allClasses = await _classService.GetAllClassesAsync();
                    if (!allClasses.Any(c => c.ClassName == name))
                    {
                        var newClass = new Class 
                        { 
                            ClassName = name, 
                            ClassCode = name,
                            CourseId = firstCourse.CourseId,
                            SemesterId = firstSemester.SemesterId,
                            DayOfWeekPair = DayOfWeekPair.MonThu,
                            TimeSlot = TimeSlot.Slot1,
                            Room = "Alpha-301",
                            MaxCapacity = 30
                        };
                        await _classService.CreateClassAsync(newClass);
                    }
                }
            }

            // 6. Seed Student User and Entity
            var studentEmail = "student1@student.com";
            var existingStudentUser = await _userService.GetByEmailAsync(studentEmail);
            
            if (existingStudentUser == null)
            {
                var roleStudent = await _roleService.GetByNameAsync("Student");
                if (roleStudent != null) 
                {
                    var studentUser = new User
                    {
                        FullName = "Nguyễn Văn A",
                        Email = studentEmail,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123123"),
                        RoleId = roleStudent.RoleId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    var createdUser = await _userService.CreateAsync(studentUser);
                    
                    var studentClass = (await _classService.GetAllClassesAsync()).FirstOrDefault();
                    if (studentClass != null)
                    {
                        var newStudent = new Student
                        {
                            UserId = createdUser.UserId,
                            StudentCode = "SV001",
                            DateOfBirth = new DateTime(2000, 1, 1),
                            ClassCode = studentClass.ClassCode,
                            Major = MajorType.SoftwareEngineering,
                            FullName = studentUser.FullName,
                            Email = studentUser.Email
                        };
                        await _studentService.CreateAsync(newStudent);
                    }
                }
            }

            TempData["SuccessMessage"] = "Seed Data thành công! Tài khoản: admin/manager/teacher/student (@student.com) - Pass: 123123";
            return RedirectToAction("Login", "Auth");
        }
    }
}
