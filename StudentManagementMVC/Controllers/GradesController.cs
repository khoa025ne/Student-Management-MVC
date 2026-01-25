using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager,Teacher")]
    public class GradesController : Controller
    {
        private readonly IScoreService _scoreService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public GradesController(IScoreService scoreService, IStudentService studentService, ICourseService courseService)
        {
            _scoreService = scoreService;
            _studentService = studentService;
            _courseService = courseService;
        }

        // GET: Grades
        public async Task<IActionResult> Index()
        {
            // Hiển thị tất cả điểm số (hoặc filter theo Teacher nếu cần)
            var scores = await _scoreService.GetAllAsync();
            return View(scores);
        }

        // GET: Grades/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName");
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName");
            return View();
        }

        // POST: Grades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Score score)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _scoreService.AddOrUpdateScoreAsync(score.StudentId, score.CourseId, score.ScoreValue);
                    TempData["SuccessMessage"] = "Thêm điểm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }

        // GET: Grades/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var score = await _scoreService.GetByIdAsync(id);
            if (score == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }

        // POST: Grades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Score score)
        {
            if (id != score.ScoreId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy điểm cần sửa!";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _scoreService.AddOrUpdateScoreAsync(score.StudentId, score.CourseId, score.ScoreValue);
                    TempData["SuccessMessage"] = "Cập nhật điểm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            ViewData["StudentId"] = new SelectList(await _studentService.GetAllAsync(), "StudentId", "User.FullName", score.StudentId);
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "CourseId", "CourseName", score.CourseId);
            return View(score);
        }
        
        // GET: Grades/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
             var score = await _scoreService.GetByIdAsync(id);
             if (score == null) return NotFound();
             return View(score);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _scoreService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa điểm thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
