using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class ScheduleController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly ISemesterService _semesterService;

        public ScheduleController(
            IEnrollmentService enrollmentService, 
            IStudentService studentService,
            ISemesterService semesterService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _semesterService = semesterService;
        }

        // GET: Schedule/Index - Hiển thị lịch học của sinh viên theo tuần
        public async Task<IActionResult> Index(int? week)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var currentStudent = await _studentService.GetByUserIdAsync(userId);

            if (currentStudent == null)
            {
                return View("NoProfile");
            }

            // Lấy học kỳ đang active
            var semesters = await _semesterService.GetAllAsync();
            var activeSemester = semesters.FirstOrDefault(s => s.IsActive);

            // Lấy tất cả enrollments của student với Status = "Active" hoặc "Enrolled"
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var studentEnrollments = allEnrollments
                .Where(e => e.StudentId == currentStudent.StudentId && 
                           (e.Status == "Active" || e.Status == "Enrolled"))
                .OrderBy(e => e.Class?.Semester?.SemesterName)
                .ThenBy(e => e.Class?.DayOfWeekPair)
                .ThenBy(e => e.Class?.TimeSlot)
                .ToList();

            if (!studentEnrollments.Any())
            {
                TempData["InfoMessage"] = "Bạn chưa đăng ký lớp học nào. Vui lòng đăng ký môn học để xem lịch học.";
            }

            // Tính số tuần trong học kỳ
            int totalWeeks = 16; // Mặc định 16 tuần
            if (activeSemester != null)
            {
                var semesterDuration = (activeSemester.EndDate - activeSemester.StartDate).Days;
                totalWeeks = (semesterDuration / 7) + 1;
            }

            // Tuần hiện tại (mặc định tuần 1)
            var currentWeek = week ?? GetCurrentWeekNumber(activeSemester);
            ViewBag.CurrentWeek = currentWeek;
            ViewBag.TotalWeeks = totalWeeks;
            ViewBag.ActiveSemester = activeSemester;
            
            // Tính ngày bắt đầu và kết thúc của tuần được chọn
            if (activeSemester != null)
            {
                var weekStartDate = activeSemester.StartDate.AddDays((currentWeek - 1) * 7);
                var weekEndDate = weekStartDate.AddDays(6);
                ViewBag.WeekStartDate = weekStartDate;
                ViewBag.WeekEndDate = weekEndDate;
            }

            return View(studentEnrollments);
        }

        private int GetCurrentWeekNumber(DataAccess.Entities.Semester? semester)
        {
            if (semester == null) return 1;
            
            var today = DateTime.Now.Date;
            if (today < semester.StartDate) return 1;
            if (today > semester.EndDate) return 16;
            
            var daysSinceStart = (today - semester.StartDate).Days;
            return (daysSinceStart / 7) + 1;
        }
    }
}
