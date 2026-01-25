using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Services.Interfaces;
using DataAccess.DAO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller quản lý phân tích học tập AI
    /// </summary>
    [Authorize]
    public class AcademicAnalysisController : Controller
    {
        private readonly IAcademicAnalysisRepository _analysisRepo;
        private readonly IStudentService _studentService;
        private readonly IGeminiAIService _geminiService;

        public AcademicAnalysisController(
            IAcademicAnalysisRepository analysisRepo,
            IStudentService studentService,
            IGeminiAIService geminiService)
        {
            _analysisRepo = analysisRepo;
            _studentService = studentService;
            _geminiService = geminiService;
        }

        /// <summary>
        /// Danh sách tất cả phân tích (Admin/Manager)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var analyses = await _analysisRepo.GetAllAsync();
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
            var students = await _studentService.GetAllAsync();
            var student = students.FirstOrDefault(s => s.Email == userEmail);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên!";
                return RedirectToAction("Index", "Home");
            }

            var analyses = await _analysisRepo.GetByStudentAsync(student.StudentId);
            ViewBag.StudentName = student.FullName;
            ViewBag.StudentCode = student.StudentCode;
            
            return View("StudentAnalysis", analyses);
        }

        /// <summary>
        /// Xem phân tích của một sinh viên cụ thể (Admin/Manager)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> StudentAnalysis(int studentId)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sinh viên!";
                return RedirectToAction(nameof(Index));
            }

            var analyses = await _analysisRepo.GetByStudentAsync(studentId);
            ViewBag.StudentName = student.FullName;
            ViewBag.StudentCode = student.StudentCode;
            
            return View(analyses);
        }

        /// <summary>
        /// Tạo phân tích mới bằng AI
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAnalysis(int studentId)
        {
            try
            {
                // Gọi AI để phân tích
                var aiResult = await _geminiService.AnalyzeStudentPerformanceAsync(studentId);

                if (!aiResult.Success)
                {
                    TempData["ErrorMessage"] = $"Lỗi AI: {aiResult.ErrorMessage}";
                    return RedirectToAction(nameof(MyAnalysis));
                }

                var student = await _studentService.GetByIdAsync(studentId);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sinh viên!";
                    return RedirectToAction(nameof(MyAnalysis));
                }

                // Lưu vào database
                var analysis = new DataAccess.Entities.AcademicAnalysis
                {
                    StudentId = studentId,
                    AnalysisDate = System.DateTime.Now,
                    OverallGPA = student.OverallGPA,
                    StrongSubjectsJson = System.Text.Json.JsonSerializer.Serialize(aiResult.StrongSubjects),
                    WeakSubjectsJson = System.Text.Json.JsonSerializer.Serialize(aiResult.WeakSubjects),
                    Recommendations = aiResult.Recommendations,
                    AiModelUsed = "Gemini Flash"
                };

                await _analysisRepo.AddAsync(analysis);

                TempData["SuccessMessage"] = "Đã tạo phân tích học tập mới bằng AI thành công!";

                // Kiểm tra role để redirect phù hợp
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction(nameof(MyAnalysis));
                }
                else
                {
                    return RedirectToAction(nameof(StudentAnalysis), new { studentId });
                }
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction(nameof(MyAnalysis));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        /// <summary>
        /// Xem chi tiết một phân tích
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var analysis = await _analysisRepo.GetByIdAsync(id);
            if (analysis == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân tích!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra quyền: Student chỉ xem được của mình
            if (User.IsInRole("Student"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var students = await _studentService.GetAllAsync();
                var student = students.FirstOrDefault(s => s.Email == userEmail);

                if (student == null || analysis.StudentId != student.StudentId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem phân tích này!";
                    return RedirectToAction(nameof(MyAnalysis));
                }
            }

            return View(analysis);
        }

        /// <summary>
        /// Xóa phân tích (Admin/Manager only)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _analysisRepo.DeleteAsync(id);
                TempData["SuccessMessage"] = "Đã xóa phân tích thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
