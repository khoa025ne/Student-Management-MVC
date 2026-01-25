using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class EnrollmentsController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;
        private readonly ISemesterService _semesterService;
        private readonly IEmailService _emailService;

        public EnrollmentsController(
            IEnrollmentService enrollmentService, 
            IStudentService studentService,
            IClassService classService,
            ISemesterService semesterService,
            IEmailService emailService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _classService = classService;
            _semesterService = semesterService;
            _emailService = emailService;
        }

        // GET: Enrollments/Register
        public async Task<IActionResult> Register()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có hồ sơ sinh viên. Vui lòng liên hệ quản trị viên.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách lớp học khả dụng
            var allClasses = await _classService.GetAllAsync();
            var availableClasses = allClasses.Where(c => c.CurrentEnrollment < c.MaxCapacity).ToList();
            
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            ViewBag.Student = student;
            return View(availableClasses);
        }

        // POST: Enrollments/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int classId)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var student = await _studentService.GetByUserIdAsync(userId);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên!";
                    return RedirectToAction(nameof(Register));
                }

                // Kiểm tra đã đăng ký chưa
                var isEnrolled = await _enrollmentService.IsEnrolledAsync(student.StudentId, classId);
                if (isEnrolled)
                {
                    TempData["ErrorMessage"] = "Bạn đã đăng ký lớp học này rồi!";
                    return RedirectToAction(nameof(Register));
                }

                var enrollment = new Enrollment
                {
                    StudentId = student.StudentId,
                    ClassId = classId,
                    EnrollmentDate = DateTime.Now,
                    Status = "Enrolled"
                };

                await _enrollmentService.CreateAsync(enrollment);

                // Gửi email xác nhận đăng ký
                try
                {
                    var classInfo = await _classService.GetByIdAsync(classId);
                    if (classInfo != null)
                    {
                        await _emailService.SendEnrollmentConfirmationAsync(
                            toEmail: student.Email,
                            studentName: student.FullName,
                            courseName: classInfo.Course?.CourseName ?? "Môn học",
                            className: classInfo.ClassName
                        );
                        TempData["SuccessMessage"] = "Đăng ký môn học thành công! Email xác nhận đã được gửi.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Đăng ký môn học thành công!";
                    }
                }
                catch (Exception emailEx)
                {
                    // Log error nhưng không fail việc đăng ký
                    TempData["WarningMessage"] = $"Đăng ký thành công nhưng gửi email thất bại: {emailEx.Message}";
                }

                return RedirectToAction(nameof(MyEnrollments));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Register));
            }
        }

        // GET: Enrollments/MyEnrollments
        public async Task<IActionResult> MyEnrollments()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var enrollments = await _enrollmentService.GetByStudentAsync(student.StudentId);
            ViewBag.Student = student;
            return View(enrollments);
        }

        // POST: Enrollments/Drop
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Drop(int enrollmentId)
        {
            try
            {
                await _enrollmentService.DeleteAsync(enrollmentId);
                TempData["SuccessMessage"] = "Hủy đăng ký môn học thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(MyEnrollments));
        }
    }
}
