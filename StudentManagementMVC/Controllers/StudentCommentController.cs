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
    [Authorize(Roles = "Teacher")]
    public class StudentCommentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IClassService _classService;
        private readonly IEmailService _emailService;

        public StudentCommentController(
            IEnrollmentService enrollmentService,
            IClassService classService,
            IEmailService emailService)
        {
            _enrollmentService = enrollmentService;
            _classService = classService;
            _emailService = emailService;
        }

        // GET: StudentComment/SelectClass
        public async Task<IActionResult> SelectClass()
        {
            try
            {
                // Lấy tất cả lớp (trong thực tế sẽ filter theo giảng viên)
                var allClasses = await _classService.GetAllAsync();
                return View(allClasses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: StudentComment/ClassStudents/5
        public async Task<IActionResult> ClassStudents(int classId)
        {
            try
            {
                var classInfo = await _classService.GetByIdAsync(classId);
                if (classInfo == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học!";
                    return RedirectToAction("SelectClass");
                }

                var enrollments = await _enrollmentService.GetByClassAsync(classId);

                var model = (classInfo, enrollments.Select(e => (
                    e,
                    e.Student,
                    new Score { ScoreValue = e.TotalScore ?? 0 }
                )).AsEnumerable());

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("SelectClass");
            }
        }

        // GET: StudentComment/EditComment/5
        public async Task<IActionResult> EditComment(int enrollmentId)
        {
            try
            {
                var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                {
                    return NotFound();
                }

                // Tạo model để lưu comment
                var commentModel = new StudentCommentModel
                {
                    EnrollmentId = enrollmentId,
                    StudentName = enrollment.Student?.FullName ?? "Unknown",
                    StudentCode = enrollment.Student?.StudentCode ?? "Unknown",
                    ClassName = enrollment.Class?.ClassName ?? "Unknown",
                    CourseName = enrollment.Class?.Course?.CourseName ?? "Unknown",
                    CurrentComment = enrollment.Comment ?? "",
                    StudentEmail = enrollment.Student?.Email ?? string.Empty,
                    Score = enrollment.TotalScore,
                    Grade = enrollment.Grade ?? string.Empty
                };

                return View(commentModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("SelectClass");
            }
        }

        // POST: StudentComment/SaveComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveComment(StudentCommentModel model)
        {
            try
            {
                var enrollment = await _enrollmentService.GetByIdAsync(model.EnrollmentId);
                if (enrollment == null)
                {
                    ViewBag.ErrorMessage = "Không tìm thấy đăng ký!";
                    return View("EditComment", model);
                }

                // Lưu comment vào enrollment
                enrollment.Comment = model.CurrentComment;
                await _enrollmentService.UpdateAsync(enrollment);

                if (model.SendEmail && enrollment.Student != null)
                {
                    var emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #2196F3;'>💬 Nhận Xét Từ Giảng Viên</h2>
                                <p>Xin chào <strong>{enrollment.Student.FullName}</strong>,</p>
                                <p>Giảng viên của bạn đã để lại nhận xét về bài làm trong môn <strong>{enrollment.Class?.Course?.CourseName}</strong>:</p>
                                
                                <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #2196F3; margin: 20px 0;'>
                                    <p><strong>Điểm:</strong> {enrollment.TotalScore?.ToString("F2") ?? "N/A"} ({enrollment.Grade ?? "N/A"})</p>
                                    <hr />
                                    <p><strong>Nhận Xét:</strong></p>
                                    <p>{model.CurrentComment}</p>
                                </div>
                                
                                <p>Vui lòng xem xét những nhận xét này và cải thiện kĩ năng của bạn.</p>
                                <p style='color: #666; font-size: 12px; margin-top: 30px;'>Email này được gửi tự động. Vui lòng không trả lời email này.</p>
                            </div>
                        </body>
                        </html>
                    ";

                    await _emailService.SendEmailAsync(
                        toEmail: enrollment.Student.Email,
                        subject: $"💬 Nhận Xét Từ Giảng Viên - {enrollment.Class?.Course?.CourseName}",
                        htmlBody: emailBody
                    );
                }

                TempData["SuccessMessage"] = "✅ Nhận xét đã được lưu" + (model.SendEmail ? " và gửi email cho sinh viên!" : "!");
                return RedirectToAction("SelectClass");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View("EditComment", model);
            }
        }
    }

    // Model for student comment
    public class StudentCommentModel
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public double? Score { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string CurrentComment { get; set; } = string.Empty;
        public bool SendEmail { get; set; } = true;
    }
}
