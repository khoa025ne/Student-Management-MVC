using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
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
            var students = await _studentService.GetAllAsync();
            
            // Map student data to users for display
            var usersWithStudentInfo = users.Select(u => new
            {
                User = u,
                Student = students.FirstOrDefault(s => s.UserId == u.UserId)
            }).ToList();
            
            ViewBag.Roles = await _roleService.GetAllAsync();
            ViewBag.UsersWithStudentInfo = usersWithStudentInfo;
            
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string fullName, string phoneNumber, string? password, int roleId, DateTime? dateOfBirth, bool isActive = true, string? majorCode = null)
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
                            // Parse major code
                            var major = MajorType.Undefined;
                            if (!string.IsNullOrEmpty(majorCode))
                            {
                                if (Enum.TryParse<MajorType>(majorCode, true, out var parsedMajor))
                                {
                                    major = parsedMajor;
                                }
                            }

                            var student = new Student
                            {
                                UserId = createdUser.UserId,
                                StudentCode = studentCode, // Sử dụng auto-generated code
                                FullName = fullName,
                                Email = email,
                                PhoneNumber = phoneNumber,
                                DateOfBirth = dateOfBirth!.Value,
                                Major = major,
                                ClassCode = "Chưa phân lớp",
                                OverallGPA = 0.0,
                                CurrentTermNo = 1
                            };

                            await _studentService.CreateAsync(student);
                            TempData["InfoMessage"] = $"Hồ sơ sinh viên đã được tạo tự động. Mã SV: {studentCode} - Ngành: {major}";
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
                    // FIX: Không hiển thị mật khẩu trên UI, chỉ thông báo đã gửi email
                    TempData["SuccessMessage"] = $"Tạo người dùng thành công! Thông tin đăng nhập đã được gửi qua email: {email}";
                }
                catch (Exception emailEx)
                {
                    // Chỉ hiển thị mật khẩu khi email thất bại (cần thiết cho admin)
                    TempData["WarningMessage"] = $"Tạo người dùng thành công nhưng gửi email thất bại: {emailEx.Message}. Vui lòng cung cấp mật khẩu cho người dùng qua kênh khác.";
                    // Log mật khẩu cho admin xem trong console thay vì UI
                    Console.WriteLine($"[ADMIN] Password for {email}: {finalPassword}");
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
                TempData["ErrorMessage"] = $"Không tìm thấy người dùng với ID: {id}!";
                return RedirectToAction(nameof(Index));
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
                TempData["ErrorMessage"] = $"Không khớp ID: URL ID ({id}) khác với User ID ({user.UserId})!";
                return RedirectToAction(nameof(Index));
            }

            // Remove Role validation error (Role is navigation property, not submitted from form)
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra user tồn tại
                    var existingUser = await _userService.GetByIdAsync(id);
                    if (existingUser == null)
                    {
                        TempData["ErrorMessage"] = $"Không tìm thấy người dùng với ID: {id}!";
                        return RedirectToAction(nameof(Index));
                    }

                    // Kiểm tra email duplicate (trừ chính nó)
                    var emailExists = await _userService.GetByEmailAsync(user.Email);
                    if (emailExists != null && emailExists.UserId != user.UserId)
                    {
                        TempData["ErrorMessage"] = $"Email '{user.Email}' đã được sử dụng bởi người dùng khác!";
                        ViewBag.Roles = await _roleService.GetAllAsync();
                        return View(user);
                    }

                    await _userService.UpdateAsync(user);
                    TempData["SuccessMessage"] = $"Cập nhật người dùng '{user.FullName}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật người dùng: {ex.Message}";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                // Log validation errors for debugging
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Dữ liệu không hợp lệ! {errors}";
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
                    TempData["ErrorMessage"] = $"Không tìm thấy người dùng với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                user.IsActive = !user.IsActive;
                await _userService.UpdateAsync(user);

                TempData["SuccessMessage"] = user.IsActive 
                    ? $"Đã mở khóa tài khoản '{user.FullName}' ({user.Email})!" 
                    : $"Đã khóa tài khoản '{user.FullName}' ({user.Email})!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi thay đổi trạng thái tài khoản: {ex.Message}";
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
                    TempData["ErrorMessage"] = $"Không tìm thấy người dùng với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                // Reset to default password: ddMMyyyy@fpt (based on DateOfBirth or 01011990@fpt)
                var defaultPassword = "01011990@fpt";
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
                user.MustChangePassword = true;
                
                await _userService.UpdateAsync(user);

                TempData["SuccessMessage"] = $"Đã reset mật khẩu cho '{user.FullName}'! Mật khẩu mặc định: {defaultPassword}. Người dùng sẽ được yêu cầu đổi mật khẩu khi đăng nhập lần sau.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi reset mật khẩu: {ex.Message}";
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
                    TempData["ErrorMessage"] = $"Không tìm thấy người dùng với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                // Không cho xóa Admin
                if (user.Role?.RoleName == "Admin")
                {
                    TempData["ErrorMessage"] = $"Không thể xóa tài khoản Admin '{user.FullName}'! Tài khoản Admin không thể bị xóa khỏi hệ thống.";
                    return RedirectToAction(nameof(Index));
                }

                var userName = user.FullName;
                var userEmail = user.Email;
                
                await _userService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Xóa người dùng '{userName}' ({userEmail}) thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY") || ex.Message.Contains("foreign key"))
                {
                    TempData["ErrorMessage"] = $"Không thể xóa người dùng: {ex.Message}. Có thể người dùng này đang có dữ liệu liên quan (hồ sơ sinh viên, điểm số, thông báo...)!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa người dùng: {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Cập nhật Role cho User với tự động tạo hồ sơ
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
                var newRole = (await _roleService.GetByIdAsync(newRoleId))?.RoleName;
                
                // Cập nhật role
                user.RoleId = newRoleId;
                await _userService.UpdateAsync(user);

                // Tự động tạo hồ sơ Student nếu đổi sang role Student
                if (newRole == "Student")
                {
                    // Kiểm tra xem user đã có hồ sơ Student chưa
                    var students = await _studentService.GetAllAsync();
                    var existingStudent = students.FirstOrDefault(s => s.UserId == userId);
                    
                    if (existingStudent == null)
                    {
                        // Tạo hồ sơ Student mới với thông tin cơ bản
                        var newStudent = new Student
                        {
                            UserId = userId,
                            StudentCode = $"SV{DateTime.Now:yyyyMMddHHmmss}",
                            FullName = user.FullName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            DateOfBirth = DateTime.Now.AddYears(-20), // Default 20 tuổi
                            ClassCode = "TBD", // To Be Determined - cần cập nhật sau
                            Major = DataAccess.Enums.MajorType.Undefined,
                            User = user
                        };
                        
                        await _studentService.CreateAsync(newStudent);
                        
                        TempData["SuccessMessage"] = $"Đã cập nhật role từ '{oldRole}' sang '{newRole}' và tạo hồ sơ sinh viên (Mã SV: {newStudent.StudentCode})!";
                        TempData["InfoMessage"] = "Vui lòng cập nhật đầy đủ thông tin sinh viên (Ngành, Lớp, v.v.) trong phần Quản lý sinh viên.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"Đã cập nhật role từ '{oldRole}' sang '{newRole}' (Hồ sơ sinh viên đã tồn tại: {existingStudent.StudentCode})!";
                    }
                }
                else if (newRole == "Teacher")
                {
                    // TODO: Tạo hồ sơ Teacher nếu có Teacher entity
                    TempData["SuccessMessage"] = $"Đã cập nhật role từ '{oldRole}' sang '{newRole}' thành công!";
                    TempData["InfoMessage"] = "Lưu ý: Chưa hỗ trợ tạo tự động hồ sơ giáo viên. Vui lòng tạo thủ công nếu cần.";
                }
                else
                {
                    // Admin, Manager không cần hồ sơ
                    TempData["SuccessMessage"] = $"Đã cập nhật role từ '{oldRole}' sang '{newRole}' thành công!";
                }

                // Gửi email thông báo cho user
                try
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Thông báo thay đổi quyền hạn tài khoản",
                        $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h2 style='color: #4e73df;'>Thông báo thay đổi quyền hạn</h2>
                            <p>Xin chào <strong>{user.FullName}</strong>,</p>
                            <p>Quyền hạn tài khoản của bạn đã được cập nhật:</p>
                            <div style='background: #f8f9fc; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Quyền hạn cũ:</strong> {oldRole ?? "Chưa phân loại"}</p>
                                <p style='margin: 5px 0;'><strong>Quyền hạn mới:</strong> <span style='color: #28a745; font-weight: 600;'>{newRole}</span></p>
                                <p style='margin: 5px 0;'><strong>Thời gian:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                            </div>
                            {(newRole == "Student" ? "<p style='color: #dc3545;'><strong>Lưu ý:</strong> Hồ sơ sinh viên của bạn đã được tạo. Vui lòng đăng nhập và cập nhật đầy đủ thông tin cá nhân.</p>" : "")}
                            <p>Vui lòng đăng nhập lại để cập nhật quyền truy cập mới.</p>
                            <p>Trân trọng,<br>Ban quản trị Student Compass</p>
                        </div>
                        "
                    );
                }
                catch
                {
                    // Email gửi lỗi không ảnh hưởng đến quá trình cập nhật
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật role: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
