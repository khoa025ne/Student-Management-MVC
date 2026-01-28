using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IStudentService _studentService;

        public NotificationsController(INotificationService notificationService, IStudentService studentService)
        {
            _notificationService = notificationService;
            _studentService = studentService;
        }

        // GET: Notifications/Index - Route mặc định
        public async Task<IActionResult> Index()
        {
            // Nếu là Student, redirect đến MyNotifications
            if (User.IsInRole("Student"))
            {
                return RedirectToAction(nameof(MyNotifications));
            }
            
            // Nếu là Admin/Manager, hiển thị tất cả thông báo
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                var allNotifications = await _notificationService.GetAllAsync();
                return View("AdminIndex", allNotifications);
            }

            // Fallback về MyNotifications cho các role khác
            return RedirectToAction(nameof(MyNotifications));
        }

        // GET: Notifications/MyNotifications
        public async Task<IActionResult> MyNotifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var notifs = await _notificationService.GetMyNotificationsAsync(userId);
                return View(notifs);
            }
            return RedirectToAction("Login", "Auth");
        }

        // POST: Notifications/MarkRead/5
        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                // Kiểm tra thông báo tồn tại
                var notification = await _notificationService.GetByIdAsync(id);
                if (notification == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy thông báo với ID: {id}!";
                    return RedirectToAction(nameof(MyNotifications));
                }

                await _notificationService.MarkAsReadAsync(id);
                TempData["SuccessMessage"] = $"Đánh dấu đã đọc thông báo: {notification.Message}";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi đánh dấu đã đọc: {ex.Message}";
            }
            return RedirectToAction(nameof(MyNotifications));
        }

        // GET: Notifications/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName");
            return View();
        }

        // POST: Notifications/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification notification)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra student tồn tại
                    if (notification.StudentId.HasValue)
                    {
                        var student = await _studentService.GetByIdAsync(notification.StudentId.Value);
                        if (student == null)
                        {
                            TempData["ErrorMessage"] = $"Không tìm thấy sinh viên với ID: {notification.StudentId.Value}!";
                            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", notification.StudentId);
                            return View(notification);
                        }
                    }

                    // Validate message không rỗng
                    if (string.IsNullOrWhiteSpace(notification.Message))
                    {
                        TempData["ErrorMessage"] = "Nội dung thông báo không được để trống!";
                        ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", notification.StudentId);
                        return View(notification);
                    }

                    notification.CreatedAt = System.DateTime.Now;
                    notification.IsRead = false;
                    await _notificationService.CreateNotificationAsync(notification);
                    
                    var studentName = notification.StudentId.HasValue 
                        ? (await _studentService.GetByIdAsync(notification.StudentId.Value))?.User?.FullName 
                        : "tất cả sinh viên";
                    TempData["SuccessMessage"] = $"Gửi thông báo thành công đến {studentName}!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tạo thông báo: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại thông tin.";
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", notification.StudentId);
            return View(notification);
        }
        
         // POST: Notifications/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Kiểm tra thông báo tồn tại
                var notification = await _notificationService.GetByIdAsync(id);
                if (notification == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy thông báo với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                var message = notification.Message?.Substring(0, Math.Min(50, notification.Message.Length));
                await _notificationService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Xóa thông báo thành công! ('{message}...')";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa thông báo: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
        
        // API: Get recent notifications for dropdown
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetRecentNotifications()
        {
            try
            {
                var studentId = await GetCurrentStudentIdAsync();
                if (!studentId.HasValue)
                {
                    return Json(new List<object>());
                }

                var notifications = await _notificationService.GetMyNotificationsAsync(studentId.Value);
                var recentNotifications = notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(10)
                    .Select(n => new
                    {
                        n.NotificationId,
                        n.Title,
                        n.Message,
                        n.Type,
                        n.IsRead,
                        n.CreatedAt,
                        n.Link
                    });

                return Json(recentNotifications);
            }
            catch (System.Exception)
            {
                return Json(new List<object>());
            }
        }

        // API: Get unread notification count
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var studentId = await GetCurrentStudentIdAsync();
                if (!studentId.HasValue)
                {
                    return Json(0);
                }

                var notifications = await _notificationService.GetMyNotificationsAsync(studentId.Value);
                var unreadCount = notifications.Count(n => !n.IsRead);
                
                return Json(unreadCount);
            }
            catch (System.Exception)
            {
                return Json(0);
            }
        }

        // Helper method to get current student ID
        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var students = await _studentService.GetAllStudentsAsync();
            var student = students.FirstOrDefault(s => s.User != null && s.User.Email == userName);
            
            return student?.StudentId;
        }
    }
}
