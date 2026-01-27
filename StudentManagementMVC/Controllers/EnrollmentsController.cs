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
                TempData["ErrorMessage"] = "Bạn chưa có hồ sơ sinh viên! Vui lòng liên hệ phòng đào tạo hoặc quản trị viên để được hỗ trợ.";
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
                    TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên! Vui lòng kiểm tra lại hoặc liên hệ quản trị viên.";
                    return RedirectToAction(nameof(Register));
                }

                // Kiểm tra đã đăng ký chưa
                var isEnrolled = await _enrollmentService.IsEnrolledAsync(student.StudentId, classId);
                if (isEnrolled)
                {
                    TempData["WarningMessage"] = "Bạn đã đăng ký lớp học này rồi! Vui lòng kiểm tra danh sách lớp học của bạn.";
                    return RedirectToAction(nameof(Register));
                }

                // Kiểm tra số lượng chỗ còn trống
                var classInfo = await _classService.GetByIdAsync(classId);
                if (classInfo == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin lớp học! Vui lòng thử lại sau.";
                    return RedirectToAction(nameof(Register));
                }

                if (classInfo.CurrentEnrollment >= classInfo.MaxCapacity)
                {
                    TempData["ErrorMessage"] = $"Lớp {classInfo.ClassName} đã đầy ({classInfo.CurrentEnrollment}/{classInfo.MaxCapacity} sinh viên)! Vui lòng chọn lớp khác.";
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
                
                // UPDATE: Cập nhật sĩ số của lớp học
                classInfo.CurrentEnrollment += 1;
                await _classService.UpdateAsync(classInfo);

                // Gửi email xác nhận đăng ký
                try
                {
                    if (classInfo != null)
                    {
                        await _emailService.SendEnrollmentConfirmationAsync(
                            toEmail: student.Email,
                            studentName: student.FullName,
                            courseName: classInfo.Course?.CourseName ?? "Môn học",
                            className: classInfo.ClassName
                        );
                        TempData["SuccessMessage"] = $"Đăng ký lớp {classInfo.ClassName} - {classInfo.Course?.CourseName} thành công! Email xác nhận đã được gửi đến {student.Email}.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Đăng ký môn học thành công!";
                    }
                }
                catch (Exception emailEx)
                {
                    // Log error nhưng không fail việc đăng ký
                    TempData["WarningMessage"] = $"Đăng ký môn học thành công! Tuy nhiên, hệ thống không thể gửi email xác nhận: {emailEx.Message}";
                }

                return RedirectToAction(nameof(MyEnrollments));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đăng ký môn học thất bại: {ex.Message}. Vui lòng thử lại sau hoặc liên hệ phòng đào tạo!";
                return RedirectToAction(nameof(Register));
            }
        }

        // GET: Enrollments/MyEnrollments
        public async Task<IActionResult> MyEnrollments()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập không hợp lệ! Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên! Vui lòng liên hệ quản trị viên.";
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
                var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đăng ký!";
                    return RedirectToAction(nameof(MyEnrollments));
                }
                
                var student = enrollment.Student;
                var className = enrollment.Class?.ClassName ?? "Lớp học";
                
                await _enrollmentService.DeleteAsync(enrollmentId);
                
                // UPDATE: Cập nhật sĩ số của lớp học
                var classInfo = await _classService.GetByIdAsync(enrollment.ClassId);
                if (classInfo != null && classInfo.CurrentEnrollment > 0)
                {
                    classInfo.CurrentEnrollment -= 1;
                    await _classService.UpdateAsync(classInfo);
                }
                
                // Gửi email hủy đăng ký
                try
                {
                    if (student != null)
                    {
                        var subject = "Thông báo hủy đăng ký môn học";
                        var body = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif;'>
                                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                    <h2 style='color: #dc3545;'>⚠️ Hủy đăng ký thành công</h2>
                                    <p>Xin chào <strong>{student.FullName}</strong>,</p>
                                    <p>Bạn đã hủy đăng ký môn học:</p>
                                    <div style='background-color: #f0f0f0; padding: 15px; margin: 20px 0;'>
                                        <p><strong>Lớp học:</strong> {className}</p>
                                        <p><strong>Ngày hủy:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                                    </div>
                                    <p>Nếu bạn muốn đăng ký lại, vui lòng vào hệ thống để thực hiện. Lớp học sẽ chỉ còn dành cho các sinh viên khác.</p>
                                    <p style='color: #666; font-size: 12px;'>Email này được gửi tự động. Vui lòng không trả lời email này.</p>
                                </div>
                            </body>
                            </html>
                        ";
                        
                        await _emailService.SendEmailAsync(
                            toEmail: student.Email,
                            subject: subject,
                            htmlBody: body
                        );
                        
                        TempData["SuccessMessage"] = $"Hủy đăng ký lớp {className} thành công! Email xác nhận đã được gửi.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"Hủy đăng ký thành công!";
                    }
                }
                catch (Exception emailEx)
                {
                    TempData["WarningMessage"] = $"Hủy đăng ký thành công nhưng không gửi được email: {emailEx.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Không thể hủy đăng ký: {ex.Message}. Vui lòng thử lại hoặc liên hệ phòng đào tạo!";
            }
            return RedirectToAction(nameof(MyEnrollments));
        }
    }
}
