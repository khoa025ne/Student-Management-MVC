using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using Services.Models;
using System.Threading.Tasks;
using System.Linq;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager,Teacher")]
    public class GradesController : Controller
    {
        private readonly IScoreService _scoreService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEmailService _emailService;
        private readonly IGeminiAIService _geminiAIService;

        public GradesController(IScoreService scoreService, IStudentService studentService, ICourseService courseService, IEmailService emailService, IGeminiAIService geminiAIService)
        {
            _scoreService = scoreService;
            _studentService = studentService;
            _courseService = courseService;
            _emailService = emailService;
            _geminiAIService = geminiAIService;
        }

        // GET: Grades
        public async Task<IActionResult> Index()
        {
            // Hiển thị tất cả điểm số (hoặc filter theo Teacher nếu cần)
            var scores = await _scoreService.GetAllAsync();
            return View(scores);
        }

        // GET: Grades/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName");
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName");
            return View();
        }

        // POST: Grades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Score score)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get student and course info for email
                    var student = await _studentService.GetByIdAsync(score.StudentId);
                    var course = await _courseService.GetByIdAsync(score.CourseId);
                    
                    await _scoreService.AddOrUpdateScoreAsync(score.StudentId, score.CourseId, score.ScoreValue);
                    
                    // Gửi email thông báo điểm
                    try
                    {
                        if (student != null && course != null)
                        {
                            // Tính grade
                            var scoreValue = score.ScoreValue;
                            string grade = scoreValue >= 8.5 ? "A" : scoreValue >= 8 ? "B+" : scoreValue >= 7 ? "B" : 
                                         scoreValue >= 6 ? "C+" : scoreValue >= 5.5 ? "C" : scoreValue >= 4 ? "D" : "F";
                            
                            // Tính GPA (simplified - assuming 4.0 scale)
                            var gpa = (scoreValue / 10) * 4;
                            
                            await _emailService.SendScoreNotificationAsync(
                                toEmail: student.Email,
                                studentName: student.FullName,
                                courseName: course.CourseName,
                                gpa: gpa,
                                grade: grade
                            );
                            
                            // 🤖 THÊM: Chạy AI analysis nếu sinh viên có nhiều môn học
                            try
                            {
                                var allEnrollments = await _scoreService.GetAllAsync();
                                var studentEnrollments = allEnrollments.Where(e => e.StudentId == student.StudentId && e.TotalScore.HasValue).ToList();
                                
                                // Nếu sinh viên đã học >= 3 môn, chạy AI analysis
                                if (studentEnrollments.Count >= 3)
                                {
                                    var analysis = await _geminiAIService.AnalyzeStudentPerformanceAsync(student.StudentId);
                                    
                                    if (analysis.Success)
                                    {
                                        // Gửi email phân tích
                                        var analysisEmail = $@"
                                            <html>
                                            <body style='font-family: Arial, sans-serif;'>
                                                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                                    <h2 style='color: #6f42c1;'>🤖 AI Phân Tích Học Tập Mới</h2>
                                                    <p>Xin chào <strong>{student.FullName}</strong>,</p>
                                                    <p>Dựa trên kết quả học tập hiện tại, AI đã phân tích:</p>
                                                    
                                                    <div style='background-color: #f0f0f0; padding: 15px; margin: 20px 0; border-radius: 8px;'>
                                                        <h4 style='color: #28a745;'>✅ Điểm Mạnh:</h4>
                                                        <ul>
                                                            {string.Join("", analysis.StrongSubjects.Select(s => $"<li>{s}</li>"))}
                                                        </ul>
                                                        
                                                        <h4 style='color: #dc3545;'>⚠️ Điểm Yếu:</h4>
                                                        <ul>
                                                            {string.Join("", analysis.WeakSubjects.Select(s => $"<li>{s}</li>"))}
                                                        </ul>
                                                    </div>
                                                    
                                                    <div style='background-color: #e7f3ff; padding: 15px; border-left: 4px solid #2196F3; margin: 20px 0;'>
                                                        <h4>💡 Khuyến Nghị:</h4>
                                                        <p>{analysis.Recommendations}</p>
                                                    </div>
                                                    
                                                    <p style='color: #666; font-size: 12px;'>Email này được tạo tự động bởi hệ thống AI.</p>
                                                </div>
                                            </body>
                                            </html>
                                        ";
                                        
                                        await _emailService.SendEmailAsync(
                                            toEmail: student.Email,
                                            subject: "🤖 Phân Tích AI Kết Quả Học Tập Của Bạn",
                                            htmlBody: analysisEmail
                                        );
                                        
                                        TempData["InfoMessage"] = "✨ Email phân tích AI đã được gửi!";
                                    }
                                }
                            }
                            catch (Exception aiEx)
                            {
                                // Không fail nếu AI analysis fail
                                Console.WriteLine($"AI Analysis error: {aiEx.Message}");
                            }
                            
                            TempData["SuccessMessage"] = $"Thêm điểm thành công! Email thông báo đã được gửi đến {student.Email}";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Thêm điểm thành công!";
                        }
                    }
                    catch (Exception emailEx)
                    {
                        TempData["WarningMessage"] = $"Thêm điểm thành công nhưng không gửi được email: {emailEx.Message}";
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }

        // GET: Grades/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var score = await _scoreService.GetByIdAsync(id);
            if (score == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }

        // POST: Grades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Score score)
        {
            if (id != score.ScoreId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy điểm cần sửa!";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _scoreService.AddOrUpdateScoreAsync(score.StudentId, score.CourseId, score.ScoreValue);
                    TempData["SuccessMessage"] = "Cập nhật điểm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }
        
        // GET: Grades/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
             var score = await _scoreService.GetByIdAsync(id);
             if (score == null) return NotFound();
             return View(score);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _scoreService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa điểm thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
