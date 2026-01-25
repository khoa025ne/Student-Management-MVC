using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Repositories.Interfaces;

namespace StudentManagementMVC.Controllers
{
    public class SeedController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IRoleRepository _roleRepo;
        private readonly IClassRepository _classRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly ISemesterRepository _semesterRepo;

        public SeedController(
            IUserService userService,
            IStudentService studentService,
            IRoleRepository roleRepo,
            IClassRepository classRepo,
            ICourseRepository courseRepo,
            ISemesterRepository semesterRepo)
        {
            _userService = userService;
            _studentService = studentService;
            _roleRepo = roleRepo;
            _classRepo = classRepo;
            _courseRepo = courseRepo;
            _semesterRepo = semesterRepo;
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
                var allRoles = await _roleRepo.GetAllAsync();
                if (!allRoles.Any(r => r.RoleName == role.RoleName))
                {
                    await _roleRepo.AddAsync(role);
                }
            }

            // Helper to Create User if not exists
            async Task CreateUserIfNotExists(string email, string fullName, string password, string roleName, string phoneNumber = "0000000000")
            {
                var existingUser = await _userService.GetByEmailAsync(email);
                if (existingUser == null)
                {
                    var allRoles = await _roleRepo.GetAllAsync();
                    var role = allRoles.FirstOrDefault(r => r.RoleName == roleName);
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
            
            // Student needs special handling for Student entity, check below

            // 3. Seed Semesters (Moved up)
            var semesterData = new[] 
            { 
                new { Name = "Học kỳ 1 - 2024", Code = "FA24" }, 
                new { Name = "Học kỳ 2 - 2024", Code = "SP25" } 
            };

            foreach (var item in semesterData)
            {
                var all = await _semesterRepo.GetAllAsync();
                if (!all.Any(s => s.SemesterName == item.Name))
                {
                    await _semesterRepo.AddAsync(new Semester 
                    { 
                        SemesterName = item.Name,
                        SemesterCode = item.Code,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(4),
                        IsActive = true
                    });
                }
            }

            // 4. Seed Courses (Moved up)
            var courses = new[] 
            { 
                new Course { CourseName = "Lập trình .NET", CourseCode = "PRN211", Credits = 3, Major = "CNTT" },
                new Course { CourseName = "Cấu trúc dữ liệu", CourseCode = "CSD201", Credits = 4, Major = "CNTT" },
                new Course { CourseName = "Cơ sở dữ liệu", CourseCode = "DBI202", Credits = 3, Major = "CNTT" }
            };
            foreach (var c in courses)
            {
                var all = await _courseRepo.GetAllAsync();
                if (!all.Any(x => x.CourseName == c.CourseName))
                {
                    await _courseRepo.AddAsync(c);
                }
            }

            // 5. Seed Classes (Now has dependencies)
            // Get dependencies first
            var firstSemester = (await _semesterRepo.GetAllAsync()).FirstOrDefault();
            var firstCourse = (await _courseRepo.GetAllAsync()).FirstOrDefault();

            if (firstSemester != null && firstCourse != null)
            {
                var classNames = new[] { "SE1801", "SE1802", "SE1803" }; // Changed from K65 to match ClassCode format
                foreach (var name in classNames)
                {
                    var all = await _classRepo.GetAllAsync();
                    if (!all.Any(c => c.ClassName == name))
                    {
                        await _classRepo.AddAsync(new Class 
                        { 
                            ClassName = name, 
                            ClassCode = name,
                            CourseId = firstCourse.CourseId,
                            SemesterId = firstSemester.SemesterId,
                            // Set defaults for complex types
                            DayOfWeekPair = DataAccess.Enums.DayOfWeekPair.MonThu,
                            TimeSlot = DataAccess.Enums.TimeSlot.Slot1,
                            Room = "Alpha-301",
                            MaxCapacity = 30
                        });
                    }
                }
            }

            // 6. Seed Student User and Entity
            var studentEmail = "student1@student.com";
            var existingStudentUser = await _userService.GetByEmailAsync(studentEmail);
            
            if (existingStudentUser == null)
            {
                var roleStudent = (await _roleRepo.GetAllAsync()).FirstOrDefault(r => r.RoleName == "Student");
                if (roleStudent != null) 
                {
                    var studentUser = new User
                    {
                        FullName = "Nguyễn Văn A",
                        Email = studentEmail,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123123"),
                        RoleId = roleStudent.RoleId,
                        IsActive = true
                    };
                    var createdUser = await _userService.CreateAsync(studentUser);
                    
                    var studentClass = (await _classRepo.GetAllAsync()).FirstOrDefault();
                    if (studentClass != null)
                    {
                        var newStudent = new Student
                        {
                            UserId = createdUser.UserId,
                            StudentCode = "SV001",
                            DateOfBirth = new DateTime(2000, 1, 1),
                            ClassCode = studentClass.ClassCode,
                            Major = MajorType.SoftwareEngineering,
                            FullName = studentUser.FullName, // Ensure FullName is synced
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
