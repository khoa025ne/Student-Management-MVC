using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
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
        public async Task<IActionResult> Create(SemesterCreateDto semesterDto)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra duplicate SemesterName
                var allSemesters = await _semesterService.GetAllAsync();
                if (allSemesters.Any(s => s.SemesterName == semesterDto.SemesterName))
                {
                    TempData["ErrorMessage"] = $"❌ Lỗi: Tên học kỳ '{semesterDto.SemesterName}' đã tồn tại trong hệ thống! Vui lòng sử dụng tên khác hoặc kiểm tra lại dữ liệu.";
                    return View(semesterDto);
                }

                if (semesterDto.StartDate >= semesterDto.EndDate)
                {
                    TempData["ErrorMessage"] = $"❌ Lỗi: Ngày bắt đầu ({semesterDto.StartDate:dd/MM/yyyy}) phải trước ngày kết thúc ({semesterDto.EndDate:dd/MM/yyyy})! Vui lòng điều chỉnh lại.";
                    return View(semesterDto);
                }

                // Kiểm tra overlap với các học kỳ khác
                var overlappingSemesters = allSemesters.Where(s => 
                    (semesterDto.StartDate >= s.StartDate && semesterDto.StartDate <= s.EndDate) ||
                    (semesterDto.EndDate >= s.StartDate && semesterDto.EndDate <= s.EndDate) ||
                    (semesterDto.StartDate <= s.StartDate && semesterDto.EndDate >= s.EndDate)
                ).ToList();

                if (overlappingSemesters.Any())
                {
                    var overlappingNames = string.Join(", ", overlappingSemesters.Select(s => s.SemesterName));
                    TempData["WarningMessage"] = $"⚠️ Cảnh báo: Học kỳ mới chồng lấp thời gian với: {overlappingNames}. Vui lòng kiểm tra lại hoặc tiếp tục nếu đúng ý định.";
                }

                try
                {
                    await _semesterService.CreateDtoAsync(semesterDto);
                    TempData["SuccessMessage"] = $"Tạo học kỳ '{semesterDto.SemesterName}' thành công! ({semesterDto.StartDate:dd/MM/yyyy} - {semesterDto.EndDate:dd/MM/yyyy})";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tạo học kỳ: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại thông tin.";
            }
            return View(semesterDto);
        }

        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Console.WriteLine($"[GET Edit] Đang tải học kỳ ID: {id}");
                var semester = await _semesterService.GetByIdAsync(id);
                if (semester == null)
                {
                    Console.WriteLine($"[GET Edit] Không tìm thấy học kỳ ID: {id}");
                    TempData["ErrorMessage"] = $"Không tìm thấy học kỳ với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }
                Console.WriteLine($"[GET Edit] Đã tải học kỳ: {semester.SemesterName}");
                return View(semester);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GET Edit] Lỗi: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi tải học kỳ: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Semesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SemesterUpdateDto semesterDto)
        {
            Console.WriteLine($"[POST Edit] Bắt đầu cập nhật học kỳ ID: {id}");
            
            try
            {
                if (id != semesterDto.SemesterId)
                {
                    Console.WriteLine($"[POST Edit] Lỗi: ID không khớp - URL: {id}, Model: {semesterDto.SemesterId}");
                    TempData["ErrorMessage"] = $"Không khớp ID: URL ID ({id}) khác với Semester ID ({semesterDto.SemesterId})!";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine($"[POST Edit] ModelState không hợp lệ");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"[POST Edit] Validation Error: {error.ErrorMessage}");
                    }
                    TempData["ErrorMessage"] = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại thông tin.";
                    return View(semesterDto);
                }

                // Kiểm tra duplicate SemesterName (trừ chính nó)
                var allSemesters = await _semesterService.GetAllAsync();
                if (allSemesters.Any(s => s.SemesterName == semesterDto.SemesterName && s.SemesterId != semesterDto.SemesterId))
                {
                    Console.WriteLine($"[POST Edit] Tên học kỳ '{semesterDto.SemesterName}' đã tồn tại");
                    TempData["ErrorMessage"] = $"❌ Tên học kỳ '{semesterDto.SemesterName}' đã được sử dụng bởi học kỳ khác!";
                    return View(semesterDto);
                }

                if (semesterDto.StartDate >= semesterDto.EndDate)
                {
                    Console.WriteLine($"[POST Edit] Ngày không hợp lệ - Start: {semesterDto.StartDate}, End: {semesterDto.EndDate}");
                    TempData["ErrorMessage"] = $"❌ Ngày bắt đầu ({semesterDto.StartDate:dd/MM/yyyy}) phải trước ngày kết thúc ({semesterDto.EndDate:dd/MM/yyyy})!";
                    return View(semesterDto);
                }

                Console.WriteLine($"[POST Edit] Đang gọi UpdateDtoAsync...");
                await _semesterService.UpdateDtoAsync(semesterDto);
                Console.WriteLine($"[POST Edit] UpdateDtoAsync hoàn thành");
                
                TempData["SuccessMessage"] = $"✅ Cập nhật học kỳ '{semesterDto.SemesterName}' thành công!";
                Console.WriteLine($"[POST Edit] Thành công - Redirect về Index");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POST Edit] Exception: {ex.Message}");
                Console.WriteLine($"[POST Edit] StackTrace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"❌ Lỗi khi cập nhật học kỳ: {ex.Message}";
                return View(semesterDto);
            }
        }

        // GET: Semesters/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var semester = await _semesterService.GetByIdAsync(id);
            if (semester == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy học kỳ với ID: {id}!";
                return RedirectToAction(nameof(Index));
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
                // Kiểm tra học kỳ tồn tại
                var semester = await _semesterService.GetByIdAsync(id);
                if (semester == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy học kỳ với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                var semesterName = semester.SemesterName;
                await _semesterService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Xóa học kỳ '{semesterName}' thành công!";
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY") || ex.Message.Contains("foreign key"))
                {
                    TempData["ErrorMessage"] = $"Không thể xóa học kỳ: {ex.Message}. Có thể học kỳ đang có lớp học hoặc dữ liệu liên quan!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa học kỳ: {ex.Message}";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
