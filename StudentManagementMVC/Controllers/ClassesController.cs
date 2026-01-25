using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.Entities;
using System.Threading.Tasks;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;

        public ClassesController(IClassService classService, ICourseService courseService, ISemesterService semesterService)
        {
            _classService = classService;
            _courseService = courseService;
            _semesterService = semesterService;
        }

        // Admin, Manager: Manage all classes
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 9)
        {
            var allClasses = await _classService.GetAllAsync();
            var totalItems = allClasses.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Đảm bảo page hợp lệ
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
            
            var classes = allClasses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            
            return View(classes);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Class classEntity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _classService.CreateAsync(classEntity);
                    TempData["SuccessMessage"] = "Tạo lớp học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var classEntity = await _classService.GetByIdAsync(id);
            if (classEntity == null)
                return NotFound();

            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(Class classEntity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _classService.UpdateAsync(classEntity);
                    TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _classService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa lớp học thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Details(int id)
        {
            var classEntity = await _classService.GetByIdAsync(id);
            if (classEntity == null)
                return NotFound();
            return View(classEntity);
        }

        // Teacher: View their classes
        [Authorize(Roles = "Teacher")]
        public IActionResult MyClasses()
        {
            return Content("Danh sách lớp giảng dạy của Giảng viên (Đang phát triển)");
        }
    }
}
