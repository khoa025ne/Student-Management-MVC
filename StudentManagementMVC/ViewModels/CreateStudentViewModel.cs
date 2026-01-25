using System;
using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace StudentManagementMVC.ViewModels
{
    /// <summary>
    /// ViewModel cho form tạo sinh viên mới
    /// </summary>
    public class CreateStudentViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Mã lớp là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã lớp không quá 20 ký tự")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chuyên ngành là bắt buộc")]
        public MajorType Major { get; set; }

        public int? CurrentTermNo { get; set; } = 1;

        // Mật khẩu sẽ được sinh tự động: NgaySinh@fpt (ddMMyyyy@fpt)
        // StudentCode sẽ được sinh tự động: STU + Năm + Số thứ tự
    }
}
