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

        public ScheduleController(IEnrollmentService enrollmentService, IStudentService studentService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
        }

        // GET: Schedule/Index - Hiển thị lịch học của sinh viên
        public async Task<IActionResult> Index()
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

            // Lấy tất cả enrollments của student với Status = "Active"
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var studentEnrollments = allEnrollments
                .Where(e => e.StudentId == currentStudent.StudentId && e.Status == "Active")
                .OrderBy(e => e.Class.DayOfWeekPair)
                .ThenBy(e => e.Class.TimeSlot)
                .ToList();

            if (!studentEnrollments.Any())
            {
                TempData["InfoMessage"] = "Bạn chưa đăng ký lớp học nào. Vui lòng đăng ký môn học để xem lịch học.";
            }

            return View(studentEnrollments);
        }
    }
}
