using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: Dashboard/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var statistics = await _dashboardService.GetDashboardStatisticsAsync();
                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải thống kê: {ex.Message}";
                return View();
            }
        }

        // GET: Dashboard/GetStatistics (API for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _dashboardService.GetDashboardStatisticsAsync();
                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Dashboard/GetStatisticsByMajor (API for filtering)
        [HttpGet]
        public async Task<IActionResult> GetStatisticsByMajor(string major)
        {
            try
            {
                var statistics = await _dashboardService.GetDashboardStatisticsByMajorAsync(major);
                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Dashboard/GetStatisticsBySemester
        [HttpGet]
        public async Task<IActionResult> GetStatisticsBySemester(int semesterId)
        {
            try
            {
                var statistics = await _dashboardService.GetDashboardStatisticsBySemesterAsync(semesterId);
                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
