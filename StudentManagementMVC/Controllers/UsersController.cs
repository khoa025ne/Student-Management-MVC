using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.Entities;
using System.Threading.Tasks;
using System.Linq;
using DataAccess.Enums;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IEmailService _emailService;
        private readonly IStudentService _studentService;

        public UsersController(IUserService userService, IRoleService roleService, IEmailService emailService, IStudentService studentService)
        {
            _userService = userService;
            _roleService = roleService;
            _emailService = emailService;
            _studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string fullName, string phoneNumber, string? password, int roleId, DateTime? dateOfBirth, bool isActive = true)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _userService.GetByEmailAsync(email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại trong hệ thống!";
                    ViewBag.Roles = await _roleService.GetAllAsync();
                    return View();
                }

                // Validate số điện thoại
                if (!string.IsNullOrEmpty(phoneNumber) && !System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^0[0-9]{9}$"))
                {
                    TempData["ErrorMessage"] = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số!";
                    ViewBag.Roles = await _roleService.GetAllAsync();
                    return View();
                }

                var role = await _roleService.GetByIdAsync(roleId);
                string finalPassword;
                string studentCode = "";

                // Nếu là Student -> auto-generate password từ ngày sinh và studentCode
                if (role != null && role.RoleName == "Student")
                {
                    if (!dateOfBirth.HasValue)
                    {
                        TempData["ErrorMessage"] = "Sinh viên phải có ngày sinh!";
                        ViewBag.Roles = await _roleService.GetAllAsync();
                        return View();
                    }

                    // Validate tuổi 16-60
                    var age = DateTime.Now.Year - dateOfBirth.Value.Year;
                    if (dateOfBirth.Value > DateTime.Now.AddYears(-age)) age--;
                    if (age < 16 || age > 60)
                    {
                        TempData["ErrorMessage"] = "Tuổi sinh viên phải từ 16 đến 60!";
                        ViewBag.Roles = await _roleService.GetAllAsync();
                        return View();
                    }

                    // Auto-generate password: DDMMyyyy@fpt
                    finalPassword = _studentService.GenerateDefaultPassword(dateOfBirth.Value);
                    // Auto-generate studentCode: STU + Year + Sequence
                    studentCode = await _studentService.GenerateStudentCodeAsync();
                }
                else
                {
                    // Non-Student: validate password complexity
                    if (string.IsNullOrEmpty(password) || password.Length < 8)
                    {
                        TempData["ErrorMessage"] = "Mật khẩu phải có ít nhất 8 ký tự!";
                        ViewBag.Roles = await _roleService.GetAllAsync();
                        return View();
                    }

                    // Validate password complexity
                    bool hasUpper = password.Any(char.IsUpper);
                    bool hasLower = password.Any(char.IsLower);
                    bool hasNumber = password.Any(char.IsDigit);
                    bool hasSpecial = password.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c));

                    if (!hasUpper || !hasLower || !hasNumber || !hasSpecial)
                    {
                        TempData["ErrorMessage"] = "Mật khẩu phải có chữ HOA, thường, số và ký tự đặc biệt!";
                        ViewBag.Roles = await _roleService.GetAllAsync();
                        return View();
                    }

                    finalPassword = password;
                }

                var user = new User
                {
                    Email = email,
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(finalPassword),
                    RoleId = roleId,
                    IsActive = isActive,
                    MustChangePassword = true,
                    CreatedAt = DateTime.Now
                };

                await _userService.CreateAsync(user);

                // Tự động tạo hồ sơ sinh viên nếu role là Student
                if (role != null && role.RoleName == "Student")
                {
                    try
                    {
                        // Lấy user vừa tạo để có UserId
                        var createdUser = await _userService.GetByEmailAsync(email);
                        if (createdUser != null)
                        {
                            var student = new Student
                            {
                                UserId = createdUser.UserId,
                                StudentCode = studentCode, // Sử dụng auto-generated code
                                FullName = fullName,
                                Email = email,
                                PhoneNumber = phoneNumber,
                                DateOfBirth = dateOfBirth!.Value,
                                Major = MajorType.Undefined,
                                ClassCode = "Chưa phân lớp",
                                OverallGPA = 0.0,
                                CurrentTermNo = 1
                            };

                            await _studentService.CreateAsync(student);
                            TempData["InfoMessage"] = $"Hồ sơ sinh viên đã được tạo tự động. Mã SV: {studentCode}";
                        }
                    }
                    catch (Exception studentEx)
                    {
                        TempData["WarningMessage"] = $"User đã tạo nhưng tạo hồ sơ sinh viên thất bại: {studentEx.Message}";
                    }
                }

                // Gửi email chào mừng với mật khẩu
                try
                {
                    await _emailService.SendWelcomeEmailAsync(
                        toEmail: email,
                        studentName: fullName,
                        studentCode: string.IsNullOrEmpty(studentCode) ? email.Split('@')[0] : studentCode,
                        tempPassword: finalPassword
                    );
                    TempData["SuccessMessage"] = $"Tạo người dùng thành công! Email chào mừng đã được gửi. Mật khẩu: {finalPassword}";
                }
                catch (Exception emailEx)
                {
                    TempData["WarningMessage"] = $"Tạo người dùng thành công nhưng gửi email thất bại: {emailEx.Message}. Mật khẩu: {finalPassword}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                ViewBag.Roles = await _roleService.GetAllAsync();
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateAsync(user);
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.IsActive = !user.IsActive;
                await _userService.UpdateAsync(user);

                TempData["SuccessMessage"] = user.IsActive 
                    ? "Đã mở khóa tài khoản!" 
                    : "Đã khóa tài khoản!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Reset to default password: ddMMyyyy@fpt (based on DateOfBirth or 01011990@fpt)
                var defaultPassword = "01011990@fpt";
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
                user.MustChangePassword = true;
                
                await _userService.UpdateAsync(user);

                TempData["SuccessMessage"] = $"Đã reset mật khẩu! Mật khẩu mặc định: {defaultPassword}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Index));
                }

                // Không cho xóa Admin
                if (user.Role?.RoleName == "Admin")
                {
                    TempData["ErrorMessage"] = "Không thể xóa tài khoản Admin!";
                    return RedirectToAction(nameof(Index));
                }

                await _userService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Cập nhật Role cho User
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int userId, int newRoleId)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Index));
                }

                // Không cho thay đổi role của Admin
                if (user.Role?.RoleName == "Admin")
                {
                    TempData["ErrorMessage"] = "Không thể thay đổi role của Admin!";
                    return RedirectToAction(nameof(Index));
                }

                var oldRole = user.Role?.RoleName;
                user.RoleId = newRoleId;
                await _userService.UpdateAsync(user);

                var newRole = (await _roleService.GetByIdAsync(newRoleId))?.RoleName;
                TempData["SuccessMessage"] = $"Đã cập nhật role từ '{oldRole}' sang '{newRole}' thành công!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
