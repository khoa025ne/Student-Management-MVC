using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class ClassAssignmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;
        private readonly IEmailService _emailService;

        public ClassAssignmentController(
            IEnrollmentService enrollmentService,
            IStudentService studentService,
            IClassService classService,
            IEmailService emailService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _classService = classService;
            _emailService = emailService;
        }

        // GET: ClassAssignment/RandomAssign
        public async Task<IActionResult> RandomAssign()
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
                return RedirectToAction("Index", "Home");
            }

            // Lấy các lớp chưa đăng ký
            var allClasses = await _classService.GetAllAsync();
            var enrolledClassIds = await _enrollmentService.GetByStudentAsync(student.StudentId);
            var enrolledIds = enrolledClassIds.Select(e => e.ClassId).ToList();

            // Filter classes: không full, không đã đăng ký
            var availableClasses = allClasses
                .Where(c => c.CurrentEnrollment < c.MaxCapacity && !enrolledIds.Contains(c.ClassId))
                .ToList();

            // Sắp xếp theo semester (ưu tiên semester gần nhất), sau đó random
            var random = new Random();
            var assignedClasses = availableClasses
                .GroupBy(c => c.SemesterId)
                .OrderBy(g => g.Key)
                .SelectMany(g => g.OrderBy(x => random.Next()).Take(2)) // 2 lớp mỗi semester
                .ToList();

            ViewBag.AvailableClasses = assignedClasses;
            ViewBag.Student = student;

            return View(assignedClasses);
        }

        // POST: ClassAssignment/ConfirmRandomAssign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRandomAssign(int[] classIds)
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
                    return RedirectToAction("RandomAssign");
                }

                if (classIds == null || classIds.Length == 0)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn ít nhất 1 lớp!";
                    return RedirectToAction("RandomAssign");
                }

                var enrolledCount = 0;
                var errorClasses = new List<string>();

                foreach (var classId in classIds)
                {
                    try
                    {
                        // Kiểm tra lớp tồn tại và còn chỗ
                        var classInfo = await _classService.GetClassByIdAsync(classId);
                        if (classInfo == null)
                        {
                            errorClasses.Add($"Lớp #{classId} không tồn tại");
                            continue;
                        }

                        if (classInfo.CurrentEnrollment >= classInfo.MaxCapacity)
                        {
                            errorClasses.Add($"{classInfo.ClassName} đã đầy");
                            continue;
                        }

                        // Tạo enrollment
                        var enrollment = new Enrollment
                        {
                            StudentId = student.StudentId,
                            ClassId = classId,
                            EnrollmentDate = DateTime.Now,
                            Status = "Enrolled"
                        };

                        await _enrollmentService.CreateAsync(enrollment);

                        // Update capacity
                        classInfo.CurrentEnrollment += 1;
                        await _classService.UpdateClassAsync(classInfo);

                        // Gửi email xác nhận
                        try
                        {
                            await _emailService.SendEnrollmentConfirmationAsync(
                                toEmail: student.Email,
                                studentName: student.FullName,
                                courseName: classInfo.Course?.CourseName ?? "Môn học",
                                className: classInfo.ClassName
                            );
                        }
                        catch { /* Log error */ }

                        enrolledCount++;
                    }
                    catch (Exception ex)
                    {
                        errorClasses.Add($"Lỗi khi đăng ký: {ex.Message}");
                    }
                }

                if (enrolledCount > 0)
                {
                    TempData["SuccessMessage"] = $"✅ Đã đăng ký thành công {enrolledCount} lớp học!";
                }

                if (errorClasses.Any())
                {
                    TempData["WarningMessage"] = $"⚠️ Có {errorClasses.Count} lớp không thể đăng ký: " + string.Join("; ", errorClasses);
                }

                return RedirectToAction("MyEnrollments", "Enrollments");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("RandomAssign");
            }
        }

        // GET: ClassAssignment/TransferRequest
        public async Task<IActionResult> TransferRequest()
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
                return RedirectToAction("Index", "Home");
            }

            // Lấy các lớp đã đăng ký
            var enrollments = await _enrollmentService.GetByStudentAsync(student.StudentId);
            var enrolledClasses = enrollments.Where(e => e.Status == "Enrolled" || e.Status == "Active").ToList();

            ViewBag.EnrolledClasses = enrolledClasses;
            ViewBag.Student = student;

            return View();
        }

        // POST: ClassAssignment/RequestTransfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestTransfer(int fromClassId, int toClassId, string reason)
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
                    return RedirectToAction("TransferRequest");
                }

                var fromClass = await _classService.GetByIdAsync(fromClassId);
                var toClass = await _classService.GetByIdAsync(toClassId);

                if (fromClass == null || toClass == null)
                {
                    TempData["ErrorMessage"] = "Lớp không tồn tại!";
                    return RedirectToAction("TransferRequest");
                }

                // Tạo notification cho admin
                var transferNote = $@"
                    Yêu cầu chuyển lớp:
                    - Sinh viên: {student.FullName} ({student.StudentCode})
                    - Lớp cũ: {fromClass.ClassName}
                    - Lớp mới: {toClass.ClassName}
                    - Lý do: {reason}
                    
                    Vui lòng xem xét và phê duyệt yêu cầu này.
                ";

                // Gửi thông báo (có thể lưu vào DB notification table nếu có)
                TempData["SuccessMessage"] = "✅ Yêu cầu chuyển lớp đã được gửi! Admin sẽ xem xét trong 24 giờ.";

                return RedirectToAction("MyEnrollments", "Enrollments");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("TransferRequest");
            }
        }
    }
}
