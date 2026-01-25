using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
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
                await _notificationService.MarkAsReadAsync(id);
                TempData["SuccessMessage"] = "Đánh dấu đã đọc thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(MyNotifications));
        }

        // Admin: Manage Notifications
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var notifs = await _notificationService.GetAllAsync();
            return View(notifs);
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
                    notification.CreatedAt = System.DateTime.Now;
                    notification.IsRead = false;
                    await _notificationService.CreateNotificationAsync(notification);
                    TempData["SuccessMessage"] = "Gửi thông báo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
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
                await _notificationService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa thông báo thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
