using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Threading.Tasks;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllAsync();
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.AllCourses = await _courseService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateDto courseDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.CreateDtoAsync(courseDto);
                    TempData["SuccessMessage"] = $"Tạo môn học '{courseDto.CourseName}' ({courseDto.CourseCode}) thành công! Số tín chỉ: {courseDto.Credits}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("CourseCode") || ex.Message.Contains("duplicate"))
                    {
                        TempData["ErrorMessage"] = $"Mã môn học '{courseDto.CourseCode}' đã tồn tại! Vui lòng sử dụng mã khác.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Không thể tạo môn học: {ex.Message}";
                    }
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại thông tin.";
            }
            ViewBag.AllCourses = await _courseService.GetAllAsync();
            return View(courseDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy môn học với ID: {id}!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AllCourses = await _courseService.GetAllAsync();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseUpdateDto courseDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateDtoAsync(courseDto);
                    TempData["SuccessMessage"] = $"Cập nhật môn học '{courseDto.CourseName}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Không thể cập nhật môn học: {ex.Message}";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại.";
            }
            ViewBag.AllCourses = await _courseService.GetAllAsync();
            return View(courseDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var course = await _courseService.GetByIdAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy môn học với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }
                
                await _courseService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Xóa môn học '{course.CourseName}' thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Không thể xóa môn học: {ex.Message}. Có thể môn học đang được sử dụng trong lớp học!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
