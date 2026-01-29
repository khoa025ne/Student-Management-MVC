using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class TransfersController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;

        public TransfersController(IEnrollmentService enrollmentService, IStudentService studentService, IClassService classService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _classService = classService;
        }

        // GET: Transfers
        public async Task<IActionResult> Index()
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
               var student = await _studentService.GetByUserIdAsync(userId);
               if(student == null) return View("NoProfile");

               var enrollments = await _enrollmentService.GetByStudentAsync(student.StudentId);
               // Filter only active ??
               return View(enrollments);
            }
            return RedirectToAction("Login", "Auth");
        }

        // GET: Transfers/Create/5 (enrollmentId)
        public async Task<IActionResult> Create(int id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            if (enrollment == null) return NotFound();

            // Check authorization (is this student's enrollment?)
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var student = await _studentService.GetByUserIdAsync(userId);
                if (student == null || enrollment.StudentId != student.StudentId) return Forbid();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            // FIX: Chỉ hiển thị các lớp cùng CourseId, loại bỏ lớp hiện tại
            var allClasses = await _classService.GetAllAsync();
            var currentCourseId = enrollment.Class?.CourseId ?? 0;
            
            // Filter: cùng CourseId, khác ClassId hiện tại, còn chỗ trống
            var availableClasses = allClasses
                .Where(c => c.CourseId == currentCourseId 
                         && c.ClassId != enrollment.ClassId 
                         && c.CurrentEnrollment < c.MaxCapacity)
                .ToList();
            
            if (!availableClasses.Any())
            {
                TempData["WarningMessage"] = "Không có lớp khác có thể chuyển đến cho môn học này!";
            }
            
            ViewData["TargetClassId"] = new SelectList(availableClasses, "ClassId", "ClassName");
            ViewData["CurrentClassName"] = enrollment.Class?.ClassName ?? "N/A";
            ViewData["CurrentCourseName"] = enrollment.Class?.Course?.CourseName ?? "N/A";
            
            return View(enrollment);
        }

        // POST: Transfers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int enrollmentId, int targetClassId)
        {
            try
            {
                await _enrollmentService.ChangeClassAsync(enrollmentId, targetClassId);
                TempData["SuccessMessage"] = "Chuyển lớp thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch(System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId);
                ViewData["TargetClassId"] = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassCode");
                return View(enrollment);
            }
        }
    }
}
