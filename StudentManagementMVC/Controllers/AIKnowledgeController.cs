using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;

namespace StudentManagementMVC.Controllers
{
    /// <summary>
    /// Controller quản lý AI Knowledge Base
    /// Chỉ Admin mới có quyền truy cập
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AIKnowledgeController : Controller
    {
        private readonly IAIKnowledgeBaseService _knowledgeService;

        public AIKnowledgeController(IAIKnowledgeBaseService knowledgeService)
        {
            _knowledgeService = knowledgeService;
        }

        /// <summary>
        /// Danh sách Knowledge Base
        /// </summary>
        public async Task<IActionResult> Index(string? category = null, string? search = null)
        {
            var items = string.IsNullOrEmpty(search) && string.IsNullOrEmpty(category)
                ? await _knowledgeService.GetAllAsync()
                : await _knowledgeService.SearchAsync(search ?? "", category);

            ViewBag.Categories = AIKnowledgeCategories.GetAllCategories();
            ViewBag.SelectedCategory = category;
            ViewBag.SearchTerm = search;
            ViewBag.Stats = await _knowledgeService.GetCountByCategoryAsync();

            return View(items);
        }

        /// <summary>
        /// Chi tiết một Knowledge item
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var item = await _knowledgeService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        /// <summary>
        /// Form tạo mới
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Categories = AIKnowledgeCategories.GetAllCategories();
            return View(new AIKnowledgeBaseModel { IsActive = true, Priority = 5, Language = "vi" });
        }

        /// <summary>
        /// Xử lý tạo mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AIKnowledgeBaseModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = AIKnowledgeCategories.GetAllCategories();
                return View(model);
            }

            model.CreatedBy = User.Identity?.Name;
            await _knowledgeService.CreateAsync(model);

            TempData["SuccessMessage"] = "Đã tạo Knowledge Base item thành công!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Form chỉnh sửa
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _knowledgeService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Categories = AIKnowledgeCategories.GetAllCategories();
            return View(item);
        }

        /// <summary>
        /// Xử lý chỉnh sửa
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AIKnowledgeBaseModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = AIKnowledgeCategories.GetAllCategories();
                return View(model);
            }

            await _knowledgeService.UpdateAsync(model);

            TempData["SuccessMessage"] = "Đã cập nhật Knowledge Base item thành công!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Xóa Knowledge item
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _knowledgeService.DeleteAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xóa Knowledge Base item thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa item này!";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Toggle trạng thái Active
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var result = await _knowledgeService.ToggleActiveAsync(id);
            return Json(new { success = result });
        }

        /// <summary>
        /// Xem AI Conversation Logs
        /// </summary>
        public IActionResult Logs()
        {
            // TODO: Implement logs view
            return View();
        }

        /// <summary>
        /// Seed initial data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            await _knowledgeService.SeedInitialKnowledgeBase();
            TempData["SuccessMessage"] = "Đã seed dữ liệu Knowledge Base thành công!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// API: Search knowledge
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string query, string? category = null)
        {
            var results = await _knowledgeService.SearchAsync(query, category, 50);
            return Json(results);
        }
    }
}
