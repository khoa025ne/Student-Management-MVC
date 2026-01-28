using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller quản lý phân tích học tập AI
    /// </summary>
    [Authorize]
    public class AcademicAnalysisController : Controller
    {
        private readonly IAcademicAnalysisService _analysisService;
        private readonly IStudentService _studentService;

        public AcademicAnalysisController(
            IAcademicAnalysisService analysisService,
            IStudentService studentService)
        {
            _analysisService = analysisService;
            _studentService = studentService;
        }

        /// <summary>
        /// Danh sách tất cả phân tích (Admin/Manager)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var analyses = await _analysisService.GetAllAnalysesAsync();
            return View(analyses);
        }

        /// <summary>
        /// Xem phân tích của sinh viên (Student chỉ xem của mình)
        /// </summary>
        public async Task<IActionResult> MyAnalysis()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng!";
                return RedirectToAction("Index", "Home");
            }

            // Tìm student theo email
            var student = await _studentService.GetByEmailAsync(userEmail);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên!";
                return RedirectToAction("Index", "Home");
            }

            var analyses = await _analysisService.GetAnalysesByStudentIdAsync(student.StudentId);
            return View(analyses);
        }
                return RedirectToAction("Index", "Home");
        /// <summary>
        /// Xem phân tích của một sinh viên cụ thể (Admin/Manager)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> StudentAnalysis(string studentId)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sinh viên!";
                return RedirectToAction(nameof(Index));
            }

            var analyses = await _analysisService.GetAnalysesByStudentIdAsync(studentId);
            ViewBag.StudentName = student.FullName;
            ViewBag.StudentCode = student.StudentCode;
            
            return View(analyses);
        }

        /// <summary>
        /// Tạo phân tích mới bằng AI
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAnalysis(string studentId, string analysisType = "gpa")
        {
            try
            {
                var student = await _studentService.GetByIdAsync(studentId);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sinh viên!";
                    return RedirectToAction(nameof(MyAnalysis));
                }

                var analysis = await _analysisService.CreateAnalysisAsync(studentId, analysisType);
                if (analysis == null)
                {
                    TempData["ErrorMessage"] = "Không thể tạo phân tích. Vui lòng thử lại!";
                    return RedirectToAction(nameof(MyAnalysis));
                }

                TempData["SuccessMessage"] = "🎯 Phân tích AI đã được tạo thành công!";
                return RedirectToAction(nameof(MyAnalysis));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(MyAnalysis));
            }
        }

        /// <summary>
        /// Xóa phân tích
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteAnalysis(int analysisId)
        {
            try
            {
                var success = await _analysisService.DeleteAnalysisAsync(analysisId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã xóa phân tích thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phân tích để xóa!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Tạo phân tích GPA
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateGpaAnalysis()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng!" });
            }

            var student = await _studentService.GetByEmailAsync(userEmail);
            if (student == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ sinh viên!" });
            }

            try
            {
                var analysis = await _analysisService.GenerateGpaAnalysisAsync(student.StudentId);
                if (analysis != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã tạo phân tích GPA thành công!",
                        analysisId = analysis.AnalysisId
                    });
                }
                
                return Json(new { success = false, message = "Không thể tạo phân tích GPA!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Tạo phân tích xu hướng học tập
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneratePerformanceAnalysis()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng!" });
            }

            var student = await _studentService.GetByEmailAsync(userEmail);
            if (student == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ sinh viên!" });
            }

            try
            {
                var analysis = await _analysisService.GeneratePerformanceTrendAnalysisAsync(student.StudentId);
                if (analysis != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã tạo phân tích xu hướng thành công!",
                        analysisId = analysis.AnalysisId
                    });
                }
                
                return Json(new { success = false, message = "Không thể tạo phân tích xu hướng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Tạo phân tích lộ trình học tập
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateLearningPathAnalysis()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng!" });
            }

            var student = await _studentService.GetByEmailAsync(userEmail);
            if (student == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ sinh viên!" });
            }

            try
            {
                var analysis = await _analysisService.GenerateLearningPathAnalysisAsync(student.StudentId);
                if (analysis != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã tạo phân tích lộ trình thành công!",
                        analysisId = analysis.AnalysisId
                    });
                }
                
                return Json(new { success = false, message = "Không thể tạo phân tích lộ trình!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// API lấy danh sách sinh viên có nguy cơ
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStudentsAtRisk()
        {
            try
            {
                var riskAnalyses = await _analysisService.GetStudentsAtRiskAsync();
                return Json(new { 
                    success = true, 
                    data = riskAnalyses.Select(a => new {
                        studentId = a.StudentId,
                        studentName = a.StudentName,
                        currentGPA = a.CurrentGPA,
                        riskLevel = a.RiskLevel,
                        lastAnalysis = a.GeneratedDate.ToString("dd/MM/yyyy")
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
