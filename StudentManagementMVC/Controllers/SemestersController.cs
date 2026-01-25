using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class SemestersController : Controller
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            var semesters = await _semesterService.GetAllAsync();
            return View(semesters);
        }

        // GET: Semesters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Semesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            if (ModelState.IsValid)
            {
                if (semester.StartDate >= semester.EndDate)
                {
                    TempData["ErrorMessage"] = "Ngày bắt đầu phải trước ngày kết thúc.";
                    return View(semester);
                }

                try
                {
                    await _semesterService.CreateAsync(semester);
                    TempData["SuccessMessage"] = "Tạo học kỳ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            return View(semester);
        }

        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var semester = await _semesterService.GetByIdAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        // POST: Semesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Semester semester)
        {
            if (id != semester.SemesterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (semester.StartDate >= semester.EndDate)
                {
                    TempData["ErrorMessage"] = "Ngày bắt đầu phải trước ngày kết thúc.";
                    return View(semester);
                }

                try
                {
                    await _semesterService.UpdateAsync(semester);
                    TempData["SuccessMessage"] = "Cập nhật học kỳ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                }
            }
            return View(semester);
        }

        // GET: Semesters/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var semester = await _semesterService.GetByIdAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        // POST: Semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _semesterService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa học kỳ thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
