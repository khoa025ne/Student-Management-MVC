using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.Entities;
using StudentManagementMVC.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public StudentsController(IStudentService studentService, IUserService userService)
        {
            _studentService = studentService;
            _userService = userService;
        }

        // GET: Students - Hiển thị tất cả users có role Student
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả users
            var allUsers = await _userService.GetAllAsync();
            
            // Filter users có role Student
            var studentUsers = allUsers.Where(u => u.Role != null && u.Role.RoleName == "Student").ToList();
            
            // Truyền cả students và student users vào view
            ViewBag.StudentUsers = studentUsers;
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        // GET: Students/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateStudentViewModel { DateOfBirth = DateTime.Today.AddYears(-18) });
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = new Student
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        DateOfBirth = model.DateOfBirth,
                        ClassCode = model.ClassCode,
                        Major = model.Major,
                        CurrentTermNo = model.CurrentTermNo ?? 1,
                        OverallGPA = 0
                    };

                    await _studentService.CreateStudentWithUserAsync(
                        student,
                        model.FullName,
                        model.Email,
                        model.PhoneNumber
                    );

                    TempData["SuccessMessage"] = $"Tạo sinh viên thành công! Mã sinh viên: {student.StudentCode}. Email chào mừng đã được gửi tới {model.Email}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Email") || ex.Message.Contains("email"))
                    {
                        TempData["ErrorMessage"] = $"Email {model.Email} đã được sử dụng! Vui lòng sử dụng email khác.";
                    }
                    else if (ex.Message.Contains("phone") || ex.Message.Contains("Phone"))
                    {
                        TempData["ErrorMessage"] = $"Số điện thoại {model.PhoneNumber} đã được sử dụng! Vui lòng sử dụng số khác.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Không thể tạo sinh viên: {ex.Message}. Vui lòng kiểm tra lại thông tin!";
                    }
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        // Sử dụng cho Popup View (trả về PartialView)
        [HttpGet]
        public async Task<IActionResult> GetStudent(int id)
        {
            if (id == 0) 
            {
                TempData["ErrorMessage"] = "ID sinh viên không hợp lệ!";
                return NotFound();
            }
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) 
            {
                TempData["ErrorMessage"] = $"Không tìm thấy sinh viên với ID: {id}!";
                return NotFound();
            }
            return Json(student); // Trả về JSON để fill form trong popup
        }

        // POST: Students/Create - DISABLED: Students tự động tạo từ Users với role Student
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Cần xử lý UserId sau này khi hoàn thiện logic 
                    student.UserId = 1; // Tạm thời hardcode để test
                    await _studentService.CreateAsync(student);
                    return Json(new { success = true, message = "Thêm sinh viên thành công" });
                }
                catch (System.Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            // Trả về lỗi validate
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
        }
        */

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return Json(new { success = false, message = "❌ Lỗi: ID không khớp! Vui lòng thử lại." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.UpdateAsync(student);
                    return Json(new { success = true, message = $"✅ Cập nhật thông tin sinh viên {student.FullName} thành công!" });
                }
                catch (System.Exception ex)
                {
                    return Json(new { success = false, message = $"❌ Không thể cập nhật sinh viên: {ex.Message}. Vui lòng kiểm tra lại thông tin!" });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "❌ Dữ liệu không hợp lệ! Vui lòng kiểm tra lại tất cả các trường.", errors = errors });
        }

        // POST: Students/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student == null)
                {
                    return Json(new { success = false, message = $"❌ Không tìm thấy sinh viên với ID: {id}!" });
                }
                await _studentService.DeleteAsync(id);
                return Json(new { success = true, message = $"✅ Xóa sinh viên {student.FullName} thành công!" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = $"❌ Không thể xóa sinh viên: {ex.Message}. Có thể sinh viên đã đăng ký lớp học hoặc có dữ liệu liên quan!" });
            }
        }
    }
}
