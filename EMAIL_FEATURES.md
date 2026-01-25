# Tài liệu Chức năng Gửi Email - Student Management MVC

## 📧 Tổng quan

Hệ thống đã tích hợp đầy đủ chức năng gửi email tự động cho các sự kiện quan trọng, giống với Web API StudentManagementSystem.

## ⚙️ Cấu hình Email

### appsettings.json
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "khoaai2009@gmail.com",
  "SenderName": "Student Management System",
  "Username": "khoaai2009@gmail.com",
  "Password": "mogdfkvarowwffih",
  "EnableSsl": true
}
```

### Thư viện sử dụng
- **MailKit 4.14.1**: Gửi email qua SMTP
- **MimeKit**: Tạo email HTML

### Đăng ký Service (Program.cs)
```csharp
// Line 33
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Line 65
builder.Services.AddScoped<IEmailService, EmailService>();
```

## 📬 Các chức năng gửi email

### 1. ✅ Tạo tài khoản người dùng (UsersController)

**Trigger:** Admin tạo user mới

**Email:** Welcome Email với thông tin đăng nhập

**Template:** `SendWelcomeEmailAsync()`

**Nội dung:**
- Mã sinh viên
- Email
- Mật khẩu tạm thời
- Lưu ý bắt buộc đổi mật khẩu
- Hướng dẫn tạo mật khẩu mạnh

**Code:**
```csharp
// UsersController.cs - Line 60-73
await _emailService.SendWelcomeEmailAsync(
    toEmail: email,
    studentName: fullName,
    studentCode: email.Split('@')[0],
    tempPassword: password
);
```

**Message:**
- Success: "Tạo người dùng thành công! Email chào mừng đã được gửi."
- Warning: "Tạo người dùng thành công nhưng gửi email thất bại: {error}"

---

### 2. ✅ Đăng ký môn học (EnrollmentsController)

**Trigger:** Student đăng ký môn học thành công

**Email:** Enrollment Confirmation

**Template:** `SendEnrollmentConfirmationAsync()`

**Nội dung:**
- Tên môn học
- Tên lớp học
- Ngày đăng ký
- Lịch học và chuẩn bị tài liệu

**Code:**
```csharp
// EnrollmentsController.cs - Line 97-109
await _emailService.SendEnrollmentConfirmationAsync(
    toEmail: student.Email,
    studentName: student.FullName,
    courseName: classInfo.Course?.CourseName ?? "Môn học",
    className: classInfo.ClassName
);
```

**Message:**
- Success: "Đăng ký môn học thành công! Email xác nhận đã được gửi."
- Warning: "Đăng ký thành công nhưng gửi email thất bại: {error}"

---

### 3. ✅ Cập nhật điểm số (EnhancedScoreService)

**Trigger:** Teacher/Admin nhập/cập nhật điểm

**Email:** Score Notification + AI Analysis

**Template:** 
- `SendScoreNotificationAsync()`
- `SendAIAnalysisNotificationAsync()`

**Nội dung:**
- GPA mới
- Grade (A, B, C, D, F)
- Màu sắc badge tương ứng
- Thông báo phân tích AI

**Code:**
```csharp
// EnhancedScoreService.cs - Line 129-135
await emailService.SendScoreNotificationAsync(
    student.Email,
    student.FullName,
    "Môn học", 
    scoreValue,
    grade
);

await emailService.SendAIAnalysisNotificationAsync(
    student.Email, 
    student.FullName
);
```

**Background Processing:**
- Chạy trong background task (không block request)
- Tính Overall GPA
- Gọi Gemini AI phân tích
- Gửi email thông báo
- Tạo in-app notification
- Kiểm tra cảnh báo học vụ

---

### 4. ✅ Cảnh báo học vụ (EnhancedScoreService)

**Trigger:** GPA dưới ngưỡng

**Email:** Academic Warning

**Template:** `SendAcademicWarningAsync()`

**Nội dung:**
- GPA hiện tại
- Lý do cảnh báo
- Khuyến nghị cải thiện
- Cảnh báo đình chỉ học tập

**Code:**
```csharp
// EnhancedScoreService.cs - Line 155-163
await emailService.SendAcademicWarningAsync(
    student.Email,
    student.FullName,
    newGPA,
    "GPA dưới 2.0"
);
```

---

### 5. ✅ Gợi ý lộ trình học tập (LearningPathService)

**Trigger:** AI tạo lộ trình học tập mới

**Email:** Learning Path Recommendation

**Template:** `SendLearningPathRecommendationAsync()`

**Nội dung:**
- Các môn học được ưu tiên
- Lý do nên học môn đó
- Chiến lược học tập

**Code:**
```csharp
await _emailService.SendLearningPathRecommendationAsync(
    student.Email,
    student.FullName
);
```

---

## 🔧 Implementation Details

### EmailService Interface (IEmailService.cs)
```csharp
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string studentName, string studentCode, string tempPassword);
    Task SendEnrollmentConfirmationAsync(string toEmail, string studentName, string courseName, string className);
    Task SendScoreNotificationAsync(string toEmail, string studentName, string courseName, double gpa, string grade);
    Task SendAIAnalysisNotificationAsync(string toEmail, string studentName);
    Task SendAcademicWarningAsync(string toEmail, string studentName, double overallGPA, string reason);
    Task SendLearningPathRecommendationAsync(string toEmail, string studentName);
}
```

### EmailSettings Model
```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}
```

### Core SendEmailAsync Method
```csharp
private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    try
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, 
            _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    catch (Exception ex)
    {
        throw new Exception($"Lỗi khi gửi email: {ex.Message}", ex);
    }
}
```

## 📊 So sánh với Web API

| Chức năng | MVC | Web API | Trạng thái |
|-----------|-----|---------|------------|
| Tạo User | ✅ | ✅ | Giống nhau |
| Đăng ký môn | ✅ | ❌ | MVC tốt hơn |
| Cập nhật điểm | ✅ | ❌ | MVC tốt hơn |
| AI Analysis | ✅ | ❌ | MVC tốt hơn |
| Cảnh báo học vụ | ✅ | ❌ | MVC tốt hơn |
| Lộ trình học tập | ✅ | ❌ | MVC tốt hơn |

**Kết luận:** MVC có nhiều chức năng email hơn API!

## 🎨 Email Templates

### Welcome Email
- Header: Blue gradient (#007bff)
- Layout: Responsive, max-width 600px
- Sections: Header, Content Box, Info Box, Warning, Footer
- Button: "Đăng nhập ngay"

### Enrollment Confirmation
- Icon: ✅ Success checkmark
- Color: Green (#28a745)
- Info: Course name, Class name, Date

### Score Notification
- Icon: 📊 Chart
- Dynamic color: Green (A), Orange (B/C), Red (F)
- Display: GPA + Grade with colored badges

### Academic Warning
- Header: Red background (#dc3545)
- Icon: ⚠️ Warning
- Highlight box: Yellow (#fff3cd)
- Urgent tone with action items

### AI Analysis
- Icon: 🤖 Robot
- Color: Purple (#6f42c1)
- Features list: Strengths, Weaknesses, Recommendations

### Learning Path
- Icon: 🎯 Target
- Color: Teal (#17a2b8)
- Content: Priority courses, Strategy

## 🧪 Testing

### Test 1: Tạo User
1. Đăng nhập admin@student.com
2. Vào Users → Create
3. Nhập thông tin user mới
4. Submit → Kiểm tra email

### Test 2: Đăng ký môn
1. Đăng nhập student1@student.com
2. Vào Enrollments → Register
3. Chọn lớp học và đăng ký
4. Kiểm tra email xác nhận

### Test 3: Nhập điểm
1. Đăng nhập teacher
2. Vào Grades → Create/Edit
3. Nhập điểm cho student
4. Student nhận 2 emails:
   - Score Notification
   - AI Analysis Notification

## 🐛 Troubleshooting

### Email không gửi được

**Nguyên nhân:**
- SMTP credentials sai
- Gmail App Password không hợp lệ
- Port 587 bị chặn
- EnableSsl = false

**Giải pháp:**
1. Kiểm tra appsettings.json
2. Tạo App Password mới tại Google Account
3. Kiểm tra firewall/antivirus
4. Bật Less Secure Apps (nếu cần)

### Email vào Spam

**Giải pháp:**
- Thêm sender vào whitelist
- Cải thiện email content
- Sử dụng domain email chính thức

### Exception handling

- Email error **KHÔNG LÀM FAIL** operation chính
- Hiển thị WarningMessage thay vì ErrorMessage
- Log error để debug

## 📝 Changelog

### 2026-01-21
- ✅ Thêm IEmailService vào UsersController
- ✅ Gửi welcome email khi tạo user
- ✅ Thêm IEmailService vào EnrollmentsController
- ✅ Gửi enrollment confirmation email
- ✅ Xác nhận EnhancedScoreService đã có email
- ✅ Build thành công
- ✅ Tài liệu hoàn chỉnh

## 🎯 Next Steps

1. Test end-to-end tất cả email flows
2. Cải thiện email templates với logo
3. Thêm email tracking (open rate)
4. Cân nhắc queue system cho volume lớn
5. Thêm email preferences cho user

---

**Developed by:** Student Management Team  
**Last Updated:** 2026-01-21  
**Version:** 1.0
