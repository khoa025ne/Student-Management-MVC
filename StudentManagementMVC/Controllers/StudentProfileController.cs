using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Enums;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentProfileController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public StudentProfileController(IStudentService studentService, IUserService userService)
        {
            _studentService = studentService;
            _userService = userService;
        }

        // GET: StudentProfile/Index - Xem profile của sinh viên
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return View("NoProfile");
            }

            return View(student);
        }

        // GET: StudentProfile/EditMajor - Trang chọn ngành
        public async Task<IActionResult> EditMajor()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return View("NoProfile");
            }

            return View(student);
        }

        // POST: StudentProfile/UpdateMajor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMajor(int majorType)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên.";
                return RedirectToAction("Index", "Home");
            }

            // Validate major
            if (!Enum.IsDefined(typeof(MajorType), majorType))
            {
                TempData["ErrorMessage"] = "Ngành học không hợp lệ.";
                return RedirectToAction("EditMajor");
            }

            var major = (MajorType)majorType;
            if (major == MajorType.Undefined)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ngành học.";
                return RedirectToAction("EditMajor");
            }

            try
            {
                student.Major = major;
                await _studentService.UpdateAsync(student);

                TempData["SuccessMessage"] = "Cập nhật ngành học thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("EditMajor");
            }
        }

        // GET: StudentProfile/Edit - Sửa toàn bộ profile
        public async Task<IActionResult> Edit()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return View("NoProfile");
            }

            return View(student);
        }

        // POST: StudentProfile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Student model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Chỉ cho phép update các trường nhất định
                student.PhoneNumber = model.PhoneNumber;
                student.DateOfBirth = model.DateOfBirth;
                student.Major = model.Major;

                await _studentService.UpdateAsync(student);

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Edit");
            }
        }
    }
}
