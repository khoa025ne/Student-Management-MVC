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

        // GET: StudentProfile/Create - Tạo hồ sơ sinh viên mới
        public async Task<IActionResult> Create()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Kiểm tra xem đã có hồ sơ chưa
            var existingStudent = await _studentService.GetByUserIdAsync(userId);
            if (existingStudent != null)
            {
                TempData["InfoMessage"] = "Bạn đã có hồ sơ sinh viên rồi.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin user để điền sẵn
            var user = await _userService.GetByIdAsync(userId);
            var newStudent = new Student
            {
                UserId = userId,
                Email = user?.Email ?? "",
                FullName = user?.FullName ?? ""
            };

            return View(newStudent);
        }

        // POST: StudentProfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Remove validation cho navigation properties
            ModelState.Remove("User");
            ModelState.Remove("Class");
            ModelState.Remove("Enrollments");
            ModelState.Remove("Scores");
            ModelState.Remove("Transfers");
            ModelState.Remove("Analyses");

            // Nếu ngày sinh không hợp lệ, dùng giá trị mặc định
            if (model.DateOfBirth == default(DateTime) || model.DateOfBirth.Year < 1900)
            {
                model.DateOfBirth = new DateTime(2000, 1, 1);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra duplicate
                var existingStudent = await _studentService.GetByUserIdAsync(userId);
                if (existingStudent != null)
                {
                    TempData["ErrorMessage"] = "Bạn đã có hồ sơ sinh viên rồi!";
                    return RedirectToAction("Index");
                }

                // Tạo student code tự động nếu chưa có
                if (string.IsNullOrEmpty(model.StudentCode))
                {
                    var allStudents = await _studentService.GetAllAsync();
                    var maxStudentNumber = allStudents
                        .Select(s => s.StudentCode)
                        .Where(code => code != null && code.StartsWith("SV"))
                        .Select(code => {
                            if (int.TryParse(code.Substring(2), out int num))
                                return num;
                            return 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();

                    model.StudentCode = $"SV{(maxStudentNumber + 1):D6}";
                }

                // Set các giá trị mặc định
                model.UserId = userId;
                model.Major = MajorType.Undefined; // Admin sẽ set sau
                model.CurrentTermNo = 1;
                model.OverallGPA = 0;
                model.ClassCode = "Chưa phân lớp";
                model.CreatedAt = DateTime.Now;
                model.IsFirstLogin = true;

                await _studentService.CreateAsync(model);

                TempData["SuccessMessage"] = "Tạo hồ sơ sinh viên thành công! Admin sẽ xem xét và cập nhật thông tin ngành học cho bạn.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo hồ sơ: {ex.InnerException?.Message ?? ex.Message}";
                return View(model);
            }
        }
    }
}
