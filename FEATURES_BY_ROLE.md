# 📋 THỐNG KÊ CHỨC NĂNG HỆ THỐNG QUẢN LÝ SINH VIÊN
## Student Management System - Features By Role

> **Cập nhật:** 30/01/2026  
> **Version:** 2.0  
> **Framework:** ASP.NET Core MVC (.NET 9.0)

---

## 📊 TỔNG QUAN HỆ THỐNG

| Role | Số Controller | Số Chức năng | Mô tả |
|------|---------------|--------------|-------|
| **Admin** | 15+ | 60+ | Quản trị toàn bộ hệ thống |
| **Manager** | 12+ | 45+ | Quản lý học vụ |
| **Staff** | 3 | 12+ | Nhân viên hỗ trợ |
| **Teacher** | 5 | 15+ | Giảng viên |
| **Student** | 10 | 25+ | Sinh viên |
| **Public** | 2 | 8 | Không cần đăng nhập |

---

## 🔓 CÔNG KHAI (Không cần đăng nhập)

### 1. Xác thực (AuthController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Đăng nhập | Form đăng nhập hệ thống |
| 2 | Đăng ký | Đăng ký tài khoản sinh viên mới |
| 3 | Đăng xuất | Thoát khỏi hệ thống |
| 4 | Đổi mật khẩu | Quên/đổi mật khẩu qua email |
| 5 | Access Denied | Trang thông báo từ chối truy cập |

### 2. Trung tâm thông báo (NotificationCenterController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem thông báo | Danh sách thông báo công khai |
| 2 | Đếm chưa đọc | API lấy số thông báo chưa đọc |

---

## 🔵 SINH VIÊN (Student Role)

### 1. Dashboard Sinh viên (StudentController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | **Dashboard** | Trang chủ với GPA, tín chỉ, lịch học, điểm gần đây |
| 2 | Thống kê GPA | Biểu đồ phân bố điểm A/B/C/D/F |
| 3 | Cảnh báo học vụ | Hiển thị khi GPA < 2.0 |
| 4 | Thao tác nhanh | 6 nút truy cập nhanh các chức năng |

### 2. Đăng ký môn học (EnrollmentsController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem danh sách lớp mở | Các lớp có thể đăng ký trong học kỳ |
| 2 | Đăng ký lớp | Đăng ký vào lớp học phần |
| 3 | Xem môn đã đăng ký | Danh sách lớp đã đăng ký |
| 4 | Hủy đăng ký | Rút đăng ký khỏi lớp |

### 3. Lịch học (ScheduleController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem lịch tuần | Thời khóa biểu theo tuần |
| 2 | Lịch hôm nay | Hiển thị các lớp hôm nay |

### 4. Điểm số (ScoresController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem bảng điểm | Điểm tất cả môn học |
| 2 | GPA tổng | Điểm trung bình tích lũy |
| 3 | GPA học kỳ | Điểm trung bình từng học kỳ |

### 5. Lộ trình học tập (LearningPathController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem lộ trình | Gợi ý môn học nên đăng ký |
| 2 | Tạo lộ trình AI | AI phân tích và đề xuất lộ trình |

### 6. Chuyển lớp (TransfersController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách lớp chuyển | Các lớp có thể chuyển đến |
| 2 | Yêu cầu chuyển lớp | Gửi yêu cầu chuyển lớp |
| 3 | Xử lý chuyển | Hệ thống xử lý chuyển lớp |

### 7. Hồ sơ sinh viên (StudentProfileController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem hồ sơ | Thông tin cá nhân |
| 2 | Cập nhật hồ sơ | Sửa thông tin |
| 3 | Chọn ngành | Đăng ký/đổi ngành học |
| 4 | Tạo hồ sơ mới | Sinh viên mới tạo hồ sơ |

### 8. Đăng ký lớp nhanh (ClassAssignmentController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Gợi ý lớp | AI gợi ý lớp phù hợp |
| 2 | Đăng ký nhiều lớp | Đăng ký hàng loạt |
| 3 | Yêu cầu chuyển lớp | Form chuyển lớp |

### 9. Phân tích học tập (AcademicAnalysisController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Phân tích cá nhân | AI phân tích kết quả học tập |
| 2 | Phân tích GPA | Xu hướng điểm số |
| 3 | Dự đoán xu hướng | AI dự đoán kết quả tương lai |
| 4 | Gợi ý lộ trình | Đề xuất môn học tiếp theo |

### 10. Thông báo (NotificationsController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Thông báo của tôi | Danh sách thông báo nhận |
| 2 | Đánh dấu đã đọc | Đọc thông báo |
| 3 | Thông báo gần đây | API thông báo mới |
| 4 | Đếm chưa đọc | Số thông báo chưa xem |

---

## 🟢 GIẢNG VIÊN (Teacher Role)

### 1. Dashboard Giảng viên (TeacherController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | **Dashboard** | Trang chủ với lớp dạy, sinh viên, lịch hôm nay |
| 2 | Thống kê lớp | Số lớp, số sinh viên |
| 3 | Lớp chưa nhập điểm | Cảnh báo các lớp cần nhập điểm |
| 4 | Thao tác nhanh | 6 nút truy cập nhanh |

### 2. Quản lý lớp giảng dạy (TeacherController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách lớp | Các lớp đang giảng dạy |
| 2 | Chi tiết lớp | Thông tin lớp + danh sách SV |
| 3 | Xem lịch dạy | Lịch giảng dạy theo tuần |

### 3. Nhập điểm (TeacherController + EnrollmentGradesController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Form nhập điểm | Nhập điểm chuyên cần, giữa kỳ, cuối kỳ |
| 2 | Lưu điểm | Lưu điểm từng sinh viên |
| 3 | Nhập điểm hàng loạt | Cập nhật nhiều SV cùng lúc |
| 4 | AI phân tích điểm | Gợi ý nhận xét từ AI |

### 4. Nhận xét sinh viên (StudentCommentController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Chọn lớp | Chọn lớp để nhận xét |
| 2 | Danh sách SV | SV trong lớp đã chọn |
| 3 | Nhập nhận xét | Form nhận xét cho SV |
| 4 | Gửi email | Tự động gửi email nhận xét |

### 5. Quản lý điểm (GradesController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem điểm | Danh sách điểm các lớp |
| 2 | Thêm điểm | Thêm điểm mới |
| 3 | Sửa điểm | Chỉnh sửa điểm |
| 4 | Xóa điểm | Xóa điểm (nếu có quyền) |

---

## 🟡 NHÂN VIÊN (Staff Role)

### 1. Quản lý sinh viên (StudentsController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách SV | Xem tất cả sinh viên |
| 2 | Thêm SV mới | Tạo hồ sơ sinh viên |
| 3 | Chi tiết SV | Xem thông tin chi tiết |
| 4 | Sửa thông tin | Cập nhật hồ sơ SV |
| 5 | Xóa SV | Xóa sinh viên khỏi hệ thống |

### 2. Quản lý học kỳ (SemestersController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách học kỳ | Xem các học kỳ |
| 2 | Thêm học kỳ | Tạo học kỳ mới |
| 3 | Sửa học kỳ | Cập nhật thông tin |
| 4 | Xóa học kỳ | Xóa học kỳ |

---

## 🟠 QUẢN LÝ (Manager Role)

### Kế thừa tất cả chức năng của Staff, thêm:

### 1. Dashboard thống kê (DashboardController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Dashboard | Thống kê tổng quan hệ thống |
| 2 | Thống kê sinh viên | Số SV theo ngành, khóa |
| 3 | Thống kê điểm | Phân bố GPA toàn trường |
| 4 | Filter theo ngành | Lọc dữ liệu theo ngành |
| 5 | Filter theo học kỳ | Lọc theo học kỳ |

### 2. Quản lý người dùng (UsersController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách Users | Xem tất cả tài khoản |
| 2 | Tạo tài khoản | Thêm user mới |
| 3 | Sửa tài khoản | Cập nhật thông tin |
| 4 | Khóa/Mở khóa | Disable/Enable tài khoản |
| 5 | Reset mật khẩu | Đặt lại mật khẩu user |
| 6 | Thay đổi Role | Chuyển đổi vai trò |
| 7 | Xóa tài khoản | Xóa user khỏi hệ thống |

### 3. Quản lý lớp học (ClassesController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách lớp | Xem tất cả lớp học |
| 2 | Tạo lớp mới | Thêm lớp học phần |
| 3 | Sửa lớp | Cập nhật thông tin lớp |
| 4 | Xóa lớp | Xóa lớp học |
| 5 | Chi tiết lớp | Xem DS sinh viên trong lớp |
| 6 | Thêm SV vào lớp | Thêm sinh viên thủ công |
| 7 | Thêm SV ngẫu nhiên | Phân bổ SV tự động |

### 4. Thông báo (NotificationsController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Tạo thông báo | Gửi thông báo đến SV/GV |
| 2 | Xóa thông báo | Xóa thông báo đã gửi |

### 5. Phân tích học vụ (AcademicAnalysisController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Xem tất cả phân tích | DS phân tích toàn hệ thống |
| 2 | Phân tích SV cụ thể | Xem phân tích của 1 SV |
| 3 | Xóa phân tích | Xóa bản ghi phân tích |
| 4 | SV có nguy cơ | API danh sách SV yếu |

---

## 🔴 QUẢN TRỊ VIÊN (Admin Role)

### Kế thừa tất cả chức năng của Manager, thêm:

### 1. Quản lý môn học (CoursesController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách môn | Xem tất cả môn học |
| 2 | Tạo môn mới | Thêm môn học |
| 3 | Sửa môn | Cập nhật thông tin môn |
| 4 | Xóa môn | Xóa môn học |
| 5 | Môn tiên quyết | Thiết lập prerequisites |

### 2. AI Knowledge Base (AIKnowledgeController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Danh sách KB | Xem knowledge base |
| 2 | Chi tiết KB | Xem nội dung item |
| 3 | Tạo KB mới | Thêm kiến thức cho AI |
| 4 | Sửa KB | Cập nhật nội dung |
| 5 | Xóa KB | Xóa knowledge item |
| 6 | Bật/tắt KB | Enable/disable item |
| 7 | AI Logs | Xem lịch sử chat AI |
| 8 | Seed dữ liệu | Tạo dữ liệu mẫu |
| 9 | Tìm kiếm KB | API search knowledge |

### 3. Teacher Dashboard (TeacherController)
| STT | Chức năng | Mô tả |
|-----|-----------|-------|
| 1 | Truy cập Dashboard GV | Xem dashboard của giáo viên |

---

## 🎨 GIAO DIỆN (UI/UX)

### Student Portal
- **Theme:** Gradient đỏ-cam (#dc2626 → #ea580c)
- **Banner:** Avatar + thông tin SV + badge mã SV, lớp, ngành
- **Cards:** Stat cards với viền màu, content cards header tối
- **Layout:** Responsive, mobile-friendly

### Teacher Portal
- **Theme:** Gradient xanh emerald (#059669 → #10b981)
- **Banner:** Avatar + thông tin GV + khoa, học kỳ
- **Cards:** Class cards với badge LT/TH
- **Alert:** Cảnh báo lớp chưa nhập điểm

### Admin Portal
- **Theme:** Gradient xanh dương chuyên nghiệp
- **Dashboard:** Charts, statistics, data tables
- **Layout:** Sidebar navigation + topbar

---

## 🔧 TÍNH NĂNG KỸ THUẬT

### 1. AI Integration
- OpenRouter API cho phân tích học tập
- Knowledge base cho gợi ý thông minh
- Conversation logging

### 2. Real-time Features
- SignalR cho thông báo real-time
- Auto-refresh dashboard data

### 3. Email Service
- Gửi email cảnh báo học vụ tự động
- Email nhận xét từ giảng viên
- Password reset via email

### 4. Background Services
- AcademicWarningBackgroundService
- Tự động kiểm tra và gửi cảnh báo GPA < 2.0

### 5. Security
- Cookie-based authentication
- Role-based authorization
- CSRF protection
- Password hashing

---

## 📈 THỐNG KÊ CODE

| Thành phần | Số lượng |
|------------|----------|
| Controllers | 24 |
| Views | 80+ |
| Models/DTOs | 50+ |
| Services | 15+ |
| Entities | 12 |
| Migrations | 25+ |

---

> **Tác giả:** Student Management Team  
> **Công nghệ:** ASP.NET Core 9.0, Entity Framework Core, MySQL, SignalR, OpenRouter AI
