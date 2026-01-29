using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller dành cho Student Dashboard - Landing Page
    /// TUÂN THỦ 3-LAYER: Chỉ gọi Service, không truy cập DataAccess trực tiếp
    /// </summary>
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IAIKnowledgeBaseService _aiService;

        public StudentController(IStudentService studentService, IAIKnowledgeBaseService aiService)
        {
            _studentService = studentService;
            _aiService = aiService;
        }

        /// <summary>
        /// Student Dashboard - Landing Page
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            // FIX: Sử dụng đúng ClaimTypes.NameIdentifier thay vì "UserId"
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập không hợp lệ!";
                return RedirectToAction("Login", "Auth");
            }
            
            var dashboard = await _studentService.GetDashboardAsync(userId);
            
            if (dashboard == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(dashboard);
        }
    }
}
