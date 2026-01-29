using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudentManagementMVC.Hubs;
using Services.Interfaces;
using Services.Models;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller cho NotificationCenter
    /// TUÂN THỦ 3-LAYER: Chỉ gọi Service, không truy cập DataAccess trực tiếp
    /// </summary>
    public class NotificationCenterController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationCenterController> _logger;

        public NotificationCenterController(
            INotificationService notificationService, 
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationCenterController> logger)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
            _logger = logger;
        }

        // GET: NotificationCenter
        public async Task<IActionResult> Index(string? type, bool? unreadOnly, int page = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var userRole = HttpContext.Session.GetString("UserRole") ?? "Student";
            
            var viewModel = await _notificationService.GetNotificationCenterAsync(userId, userRole, type, unreadOnly, page);

            return View(viewModel);
        }

        // POST: NotificationCenter/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAndReturnIdAsync(id);
            return Json(new { success = true });
        }

        // POST: NotificationCenter/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var userRole = HttpContext.Session.GetString("UserRole") ?? "Student";

            var count = await _notificationService.MarkAllAsReadAsync(userId, userRole);

            return Json(new { success = true, count });
        }

        // GET: NotificationCenter/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var classes = await _notificationService.GetClassesForNotificationAsync();

            ViewBag.Classes = classes;
            ViewBag.NotificationTypes = new[] { "Info", "Warning", "Success", "Error", "Academic", "System" };
            
            return View(new CreateNotificationDto());
        }

        // POST: NotificationCenter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNotificationDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createdBy = HttpContext.Session.GetInt32("UserId") ?? 1;
            var count = await _notificationService.CreateBulkNotificationsAsync(model, createdBy);

            TempData["Success"] = $"Đã gửi {count} thông báo thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: NotificationCenter/SendGradeAlert
        [HttpPost]
        public async Task<IActionResult> SendGradeAlert(int studentId, int classId, double score)
        {
            try
            {
                await _notificationService.SendGradeAlertAsync(studentId, classId, score);

                // Send via SignalR
                await _hubContext.Clients.User(studentId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        title = score < 4.0 ? "⚠️ Cảnh báo học vụ" : "📊 Cập nhật điểm",
                        type = score < 4.0 ? "Warning" : "Info",
                        createdAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending grade alert");
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        // API: Get unread count
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var userRole = HttpContext.Session.GetString("UserRole") ?? "Student";

            var count = await _notificationService.GetUnreadCountAsync(userId, userRole);
            return Json(new { count });
        }
    }
}
