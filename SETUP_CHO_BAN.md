# 🚀 HƯỚNG DẪN SETUP DỰ ÁN - STUDENT MANAGEMENT SYSTEM

## 📋 YÊU CẦU HỆ THỐNG

### Phần mềm cần cài đặt:
1. **.NET 9.0 SDK** - [Tải tại đây](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **MySQL Server 8.0+** - [Tải tại đây](https://dev.mysql.com/downloads/mysql/)
3. **Visual Studio 2022** hoặc **VS Code** - [VS 2022](https://visualstudio.microsoft.com/) | [VS Code](https://code.visualstudio.com/)
4. **MySQL Workbench** (tùy chọn, cho GUI) - [Tải tại đây](https://dev.mysql.com/downloads/workbench/)

---

## 📦 CÁC FILE ĐÃ NHẬN

Bạn sẽ nhận được:
- 📁 **StudentManagementMVC/** - Source code dự án
- 📄 **StudentManagementDB_[date].sql** - File backup database
- 📄 **SETUP_CHO_BAN.md** - File này
- 📄 **import-database.ps1** - Script tự động import DB (Windows)

---

## 🗄️ BƯỚC 1: IMPORT DATABASE

### Cách 1: Dùng PowerShell Script (Khuyên dùng - Nhanh nhất)

1. **Mở PowerShell** (click phải Start → Windows PowerShell)

2. **Di chuyển đến thư mục chứa file:**
   ```powershell
   cd "đường_dẫn_đến_thư_mục_StudentManagementMVC"
   ```

3. **Chạy script import:**
   ```powershell
   .\import-database.ps1
   ```

4. **Làm theo hướng dẫn:**
   - Chọn file SQL đã nhận
   - Nhập password MySQL của bạn
   - Xác nhận import

✅ **Xong!** Database đã được import với tên `studentmanagementdb`

---

### Cách 2: Dùng MySQL Workbench (Có giao diện)

1. **Mở MySQL Workbench**

2. **Kết nối MySQL:**
   - Click vào connection (Local instance MySQL)
   - Nhập password

3. **Import Database:**
   - Menu: **Server → Data Import**
   - Chọn **Import from Self-Contained File**
   - Browse đến file `StudentManagementDB_[date].sql`
   - Click **Start Import**

4. **Kiểm tra:**
   - Refresh danh sách Schemas (F5)
   - Xem database `studentmanagementdb` đã xuất hiện

---

### Cách 3: Dùng Command Line (Cho cao thủ)

```bash
mysql -u root -p < StudentManagementDB_[date].sql
```

Nhập password khi được yêu cầu.

---

## ⚙️ BƯỚC 2: CẤU HÌNH PROJECT

### 1. Mở file `appsettings.json`

Đường dẫn: `StudentManagementMVC/StudentManagementMVC/appsettings.json`

### 2. Sửa Connection String (nếu cần)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=studentmanagementdb;user=root;password=MẬT_KHẨU_MYSQL_CỦA_BẠN;charset=utf8mb4"
  }
}
```

**Chỉ cần sửa:**
- `password=` → Đổi thành password MySQL của bạn
- `user=` → Nếu bạn dùng user khác `root`
- `port=` → Nếu MySQL của bạn chạy port khác 3306

### 3. Cấu hình Email (Tùy chọn)

Nếu muốn gửi email thông báo, sửa phần `EmailSettings`:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "Student Management System",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

> **Lưu ý:** Với Gmail, bạn cần tạo **App Password**, không dùng password thường.

---

## ▶️ BƯỚC 3: CHẠY DỰ ÁN

### Cách 1: Dùng Visual Studio 2022

1. Mở file `StudentManagement.sln`
2. Chọn project **StudentManagementMVC** làm startup project
3. Nhấn **F5** hoặc click **▶ Run**

### Cách 2: Dùng VS Code hoặc Terminal

```powershell
cd StudentManagementMVC/StudentManagementMVC
dotnet restore
dotnet build
dotnet run
```

### 3. Truy cập ứng dụng

Mở trình duyệt và vào:
- **HTTPS:** https://localhost:7075
- **HTTP:** http://localhost:5000

---

## 👥 TÀI KHOẢN TEST

### 🔑 Tài khoản Admin
```
Email: admin@studentmanagement.com
Password: Admin@123
```

**Quyền:**
- Quản lý Users
- Quản lý Students, Teachers
- Quản lý Classes, Courses
- Xem báo cáo, thống kê

---

### 👨‍🏫 Tài khoản Teacher (nếu có)
```
Email: teacher@studentmanagement.com
Password: Teacher@123
```

**Quyền:**
- Xem danh sách lớp được phân công
- Nhập điểm cho sinh viên
- Xem thông tin sinh viên

---

### 👨‍🎓 Tài khoản Student (nếu có)
```
Email: student@studentmanagement.com
Password: Student@123
```

**Quyền:**
- Xem thông tin cá nhân
- Xem điểm số
- Xem lịch học
- Nhận thông báo

---

## 🛠️ XỬ LÝ LỖI THƯỜNG GẶP

### ❌ Lỗi: "Unable to connect to MySQL server"

**Nguyên nhân:** Sai thông tin kết nối hoặc MySQL chưa chạy

**Giải pháp:**
1. Kiểm tra MySQL đang chạy:
   ```powershell
   Get-Service MySQL*
   ```
2. Sửa lại `appsettings.json` (user, password, port)
3. Thử ping MySQL:
   ```powershell
   mysql -u root -p
   ```

---

### ❌ Lỗi: "Database does not exist"

**Nguyên nhân:** Chưa import database

**Giải pháp:**
- Import lại database theo **BƯỚC 1**
- Kiểm tra tên database trong MySQL Workbench

---

### ❌ Lỗi: "The SDK 'Microsoft.NET.Sdk.Web' specified could not be found"

**Nguyên nhân:** Chưa cài .NET 9.0 SDK

**Giải pháp:**
1. Tải .NET 9.0 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
2. Cài đặt
3. Kiểm tra:
   ```powershell
   dotnet --version
   ```
   Kết quả phải là `9.0.x`

---

### ❌ Lỗi: Port 7075 hoặc 5000 đã bị chiếm

**Giải pháp:**
1. Dừng process đang dùng port:
   ```powershell
   netstat -ano | findstr :7075
   taskkill /PID [PID_number] /F
   ```

2. Hoặc đổi port trong `launchSettings.json`:
   ```json
   "applicationUrl": "https://localhost:7076;http://localhost:5001"
   ```

---

### ❌ Lỗi: "Access denied for user 'root'@'localhost'"

**Nguyên nhân:** Sai password MySQL

**Giải pháp:**
- Sửa lại password trong `appsettings.json`
- Hoặc reset password MySQL:
  ```sql
  ALTER USER 'root'@'localhost' IDENTIFIED BY 'new_password';
  ```

---

## 📊 CẤU TRÚC DATABASE

Database `studentmanagementdb` có **11 bảng:**

| Bảng | Mô tả |
|------|-------|
| `Users` | Tài khoản người dùng |
| `Students` | Thông tin sinh viên |
| `Teachers` | Thông tin giáo viên |
| `Classes` | Lớp học |
| `Courses` | Môn học |
| `Enrollments` | Đăng ký môn học |
| `Scores` | Điểm số |
| `Semesters` | Học kỳ |
| `Notifications` | Thông báo |
| `NotificationReadStatus` | Trạng thái đọc thông báo |
| `__EFMigrationsHistory` | Lịch sử migrations |

---

## 🎯 TÍNH NĂNG CHÍNH

✅ **Quản lý Users:**
- Tạo, sửa, xóa tài khoản
- Phân quyền (Admin, Teacher, Student)
- Tự động tạo hồ sơ sinh viên khi đổi role

✅ **Quản lý Students:**
- CRUD sinh viên
- Import từ Excel
- Export danh sách
- Xem lịch sử điểm

✅ **Quản lý Classes & Courses:**
- Tạo lớp, môn học
- Phân công giáo viên
- Đăng ký môn học

✅ **Quản lý Điểm:**
- Nhập điểm theo lớp
- Tính điểm trung bình
- Xếp loại học lực

✅ **Hệ thống Thông báo:**
- Thông báo real-time (SignalR)
- 4 loại: Achievement, Score Update, Performance Alert, Learning Path
- Email notification
- Dropdown notification trong header

✅ **Báo cáo & Thống kê:**
- Biểu đồ phân bố điểm
- Thống kê theo học kỳ
- Dashboard tổng quan

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề:

1. **Kiểm tra lại các bước setup**
2. **Xem lại phần "Xử lý lỗi thường gặp"**
3. **Kiểm tra log trong Terminal/Console khi chạy app**
4. **Liên hệ người gửi dự án cho bạn** 😊

---

## 🎉 CHÚC BẠN SETUP THÀNH CÔNG!

Sau khi setup xong, bạn có thể:
- Đăng nhập bằng tài khoản Admin
- Khám phá các tính năng
- Tạo dữ liệu mẫu
- Custom theo nhu cầu của bạn

**Happy Coding! 🚀**

---

*Tài liệu được tạo ngày: 28/01/2026*
