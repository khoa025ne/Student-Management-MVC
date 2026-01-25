using Microsoft.AspNetCore.Http;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Enums;

namespace StudentManagementMVC.Middlewares
{
    /// <summary>
    /// Middleware buộc sinh viên phải chọn ngành học trước khi sử dụng hệ thống
    /// </summary>
    public class RequireMajorMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireMajorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStudentService studentService)
        {
            // Chỉ check cho user đã đăng nhập và có role Student
            if (context.User.Identity?.IsAuthenticated == true && 
                context.User.IsInRole("Student"))
            {
                var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    var student = await studentService.GetByUserIdAsync(userId);
                    
                    // Nếu student có hồ sơ nhưng chưa chọn ngành
                    if (student != null && student.Major == MajorType.Undefined)
                    {
                        var path = context.Request.Path.Value?.ToLower() ?? "";
                        
                        // Cho phép truy cập các trang cần thiết
                        var allowedPaths = new[] { 
                            "/studentprofile/editmajor", 
                            "/studentprofile/updatemajor",
                            "/auth/logout",
                            "/auth/changepassword"
                        };

                        bool isAllowedPath = false;
                        foreach (var allowedPath in allowedPaths)
                        {
                            if (path.StartsWith(allowedPath))
                            {
                                isAllowedPath = true;
                                break;
                            }
                        }

                        // Redirect đến trang chọn ngành nếu chưa chọn
                        if (!isAllowedPath)
                        {
                            context.Response.Redirect("/StudentProfile/EditMajor");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
