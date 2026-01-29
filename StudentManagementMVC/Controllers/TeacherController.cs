using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller cho Teacher Dashboard
    /// TUÂN THỦ 3-LAYER: Chỉ gọi Service, không truy cập DataAccess trực tiếp
    /// </summary>
    [Authorize(Roles = "Teacher,Admin")]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ITeacherService teacherService, ILogger<TeacherController> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy UserId từ Claims an toàn
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            return null;
        }

        public async Task<IActionResult> Dashboard()
        {
            // FIX: Loại bỏ hardcoded fallback UserId = 1
            var teacherId = GetCurrentUserId();
            if (!teacherId.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập không hợp lệ!";
                return RedirectToAction("Login", "Auth");
            }
            
            var dashboard = await _teacherService.GetDashboardAsync(teacherId.Value);
            
            if (dashboard == null)
            {
                TempData["WarningMessage"] = "Không tìm thấy thông tin giảng viên!";
                return RedirectToAction("Login", "Auth");
            }

            return View(dashboard);
        }

        [HttpGet]
        public async Task<IActionResult> MyClasses()
        {
            // FIX: Loại bỏ hardcoded fallback UserId = 1
            var teacherId = GetCurrentUserId();
            if (!teacherId.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập không hợp lệ!";
                return RedirectToAction("Login", "Auth");
            }
            
            var classes = await _teacherService.GetMyClassesAsync(teacherId.Value);

            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> ClassDetail(int id)
        {
            var classDetail = await _teacherService.GetClassDetailAsync(id);

            if (classDetail == null)
            {
                return NotFound();
            }

            return View(classDetail);
        }

        [HttpGet]
        public async Task<IActionResult> EnterGrades(int classId)
        {
            var classDetail = await _teacherService.GetClassForGradeEntryAsync(classId);

            if (classDetail == null)
            {
                return NotFound();
            }

            return View(classDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveGrades(int classId, List<ScoreEntryDto> scores)
        {
            var success = await _teacherService.SaveGradesAsync(classId, scores);
            
            if (success)
            {
                TempData["Success"] = "Đã lưu điểm thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi lưu điểm!";
            }
            
            return RedirectToAction("ClassDetail", new { id = classId });
        }
    }
}
