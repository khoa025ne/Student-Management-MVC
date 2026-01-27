using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using StudentManagementMVC.ViewModels;
using DataAccess.Entities; // Chỉ dùng để tạo object User cho register, không gọi trực tiếp DB
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StudentManagementMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin đăng nhập!";
                return View(model);
            }

            try
            {
                var user = await _authService.LoginAsync(model.Email, model.Password);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "❌ Tên tài khoản (email) hoặc mật khẩu không chính xác! Vui lòng kiểm tra lại và thử lại.";
                    ModelState.AddModelError("", "Thông tin đăng nhập không chính xác");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa! Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                    ModelState.AddModelError("", "Tài khoản đã bị khóa");
                    return View(model);
                }

                // Kiểm tra nếu phải đổi mật khẩu
                // Staff (RoleId=4) và Lecturer (RoleId=2) không cần đổi mk lần đầu
                if (user.MustChangePassword && user.RoleId != 2 && user.RoleId != 4) // 2=Lecturer, 4=Staff
                {
                    TempData["MustChangePasswordEmail"] = user.Email;
                    TempData["WarningMessage"] = "Đây là lần đăng nhập đầu tiên hoặc tài khoản yêu cầu đổi mật khẩu. Vui lòng đổi mật khẩu để tiếp tục!";
                    return RedirectToAction("ChangePassword");
                }

                // Tạo Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Remember me
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = $"Chào mừng {user.FullName}! Đăng nhập thành công.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi đăng nhập: {ex.Message}. Vui lòng thử lại sau!";
                ModelState.AddModelError("", "Lỗi đăng nhập: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Chỉ cho phép admin/staff tạo tài khoản qua trang này
            // Sinh viên tự đăng ký thông qua form đặc biệt hoặc bị admin/staff tạo
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = new User
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    RoleId = 3 // Student role (Admin=1, Teacher=2, Student=3)
                };

                await _authService.RegisterAsync(user, model.Password);

                TempData["SuccessMessage"] = $"Đăng ký tài khoản thành công! Xin chào {model.FullName}, vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email") || ex.Message.Contains("email"))
                {
                    TempData["ErrorMessage"] = "Email này đã được sử dụng! Vui lòng sử dụng email khác hoặc đăng nhập nếu bạn đã có tài khoản.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Đăng ký thất bại: {ex.Message}. Vui lòng kiểm tra lại thông tin và thử lại!";
                }
                ModelState.AddModelError("", "Lỗi đăng ký: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["InfoMessage"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }

        // GET: Auth/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Lấy email từ TempData nếu đến từ login
            var email = TempData["MustChangePasswordEmail"] as string;
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.Email = email;
                TempData.Keep("MustChangePasswordEmail"); // Keep for POST
            }
            return View();
        }

        // POST: Auth/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string email, string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(currentPassword) || 
                    string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
                    ViewBag.Email = email;
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
                    ViewBag.Email = email;
                    return View();
                }

                if (newPassword.Length < 8)
                {
                    TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 8 ký tự!";
                    ViewBag.Email = email;
                    return View();
                }

                // Password complexity validation
                bool hasUpper = newPassword.Any(char.IsUpper);
                bool hasLower = newPassword.Any(char.IsLower);
                bool hasNumber = newPassword.Any(char.IsDigit);
                bool hasSpecial = newPassword.Any(c => !char.IsLetterOrDigit(c));

                if (!hasUpper || !hasLower || !hasNumber || !hasSpecial)
                {
                    TempData["ErrorMessage"] = "Mật khẩu phải có ít nhất 1 chữ HOA, 1 chữ thường, 1 số và 1 ký tự đặc biệt!";
                    ViewBag.Email = email;
                    return View();
                }

                // Lấy user by email
                var userEntity = await _userService.GetByEmailAsync(email);
                if (userEntity == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tài khoản!";
                    ViewBag.Email = email;
                    return View();
                }

                // Sử dụng service method để đổi password (đã có verify password logic)
                try
                {
                    bool success = await _authService.ChangePasswordAsync(userEntity.UserId, currentPassword, newPassword);
                    
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login");
                }
                catch (UnauthorizedAccessException)
                {
                    TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác!";
                    ViewBag.Email = email;
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                ViewBag.Email = email;
                return View();
            }
        }

        /// <summary>
        /// Trang thông báo truy cập bị từ chối
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
