# 📋 KẾ HOẠCH REFACTOR - KIẾN TRÚC 3 LỚP

## 🎯 MỤC TIÊU
Đưa project về đúng kiến trúc 3 lớp:
```
┌─────────────────────────────────────────┐
│     PRESENTATION LAYER (MVC)            │
│  Controllers → Views → ViewModels       │
│  Chỉ dùng: DTOs, Services Interfaces    │
└─────────────┬───────────────────────────┘
              │ (Services.Models DTOs)
              ▼
┌─────────────────────────────────────────┐
│     BUSINESS LOGIC LAYER (Services)     │
│  Services → Interfaces → DTOs           │
│  Mapping: Entity ↔ DTO                  │
└─────────────┬───────────────────────────┘
              │ (DataAccess.Entities)
              ▼
┌─────────────────────────────────────────┐
│     DATA ACCESS LAYER (DataAccess)      │
│  DbContext → Entities → Repositories    │
│  Enums (shared across layers)           │
└─────────────────────────────────────────┘
```

---

## 📊 TÌNH TRẠNG HIỆN TẠI

### ❌ Controllers vi phạm (tạo Entity trực tiếp):

| # | Controller | Số vi phạm | Mức độ |
|---|------------|-----------|--------|
| 1 | **SeedController.cs** | 12+ | 🔴 Cao |
| 2 | **UsersController.cs** | 5 | 🔴 Cao |
| 3 | **ClassesController.cs** | 5 | 🔴 Cao |
| 4 | **StudentsController.cs** | 3 | 🟡 TB |
| 5 | **EnrollmentsController.cs** | 1 | 🟢 Thấp |
| 6 | **ClassAssignmentController.cs** | 1 | 🟢 Thấp |

### ✅ Controllers đã tốt (chỉ dùng Services):
- AcademicAnalysisController.cs
- AuthController.cs (chỉ dùng DTOs)
- DashboardController.cs
- GradesController.cs (đã refactor)
- NotificationsController.cs (đã refactor)
- ScheduleController.cs
- TeacherController.cs
- TransfersController.cs

---

## 🔧 DANH SÁCH CÔNG VIỆC

### PHASE 1: Chuẩn hóa DTOs (Services Layer)
> Đảm bảo đủ DTOs cho mọi Entity

| # | File | Trạng thái | Ghi chú |
|---|------|-----------|---------|
| 1.1 | `Services/Models/Enums.cs` | 🔄 Sửa | Chỉ re-export từ DataAccess, không duplicate |
| 1.2 | `Services/Models/UserDto.cs` | ✅ Có | Cần thêm `UserUpdateDto` |
| 1.3 | `Services/Models/StudentDto.cs` | ✅ Có | OK |
| 1.4 | `Services/Models/CourseDto.cs` | ✅ Có | Đã có Create/Update DTOs |
| 1.5 | `Services/Models/ClassDto.cs` | ✅ Có | OK |
| 1.6 | `Services/Models/EnrollmentDto.cs` | ✅ Có | OK |
| 1.7 | `Services/Models/NotificationModels.cs` | ✅ Có | Đã có DTOs |
| 1.8 | `Services/Models/RoleDto.cs` | ❌ Thiếu | Cần tạo mới |
| 1.9 | `Services/Models/TeacherDto.cs` | ❌ Thiếu | Cần tạo mới |

### PHASE 2: Cập nhật Service Interfaces
> Thêm methods nhận DTOs thay vì Entities

| # | File | Methods cần thêm |
|---|------|-----------------|
| 2.1 | `IUserService.cs` | `CreateDtoAsync(UserCreateDto)`, `UpdateDtoAsync(UserUpdateDto)` |
| 2.2 | `IStudentService.cs` | ✅ Đã có DTO methods |
| 2.3 | `ICourseService.cs` | `CreateDtoAsync`, `UpdateDtoAsync` |
| 2.4 | `ISemesterService.cs` | `CreateDtoAsync`, `UpdateDtoAsync` |
| 2.5 | `IClassService.cs` | `CreateDtoAsync`, `UpdateDtoAsync` |
| 2.6 | `IEnrollmentService.cs` | `CreateDtoAsync` |
| 2.7 | `INotificationService.cs` | `CreateNotificationDtoAsync` |

### PHASE 3: Implement Service Methods
> Thêm mapping logic trong Service Implementations

| # | File | Ghi chú |
|---|------|---------|
| 3.1 | `UserService.cs` | Implement DTO methods |
| 3.2 | `CourseService.cs` | Implement DTO methods |
| 3.3 | `SemesterService.cs` | Implement DTO methods |
| 3.4 | `ClassService.cs` | Implement DTO methods |
| 3.5 | `EnrollmentService.cs` | Implement DTO methods |
| 3.6 | `NotificationService.cs` | Implement DTO methods |

### PHASE 4: Refactor Controllers
> Sửa Controllers để dùng DTOs thay vì Entities

| # | Controller | Ưu tiên | Công việc |
|---|------------|--------|-----------|
| 4.1 | `StudentsController.cs` | 🔴 | Thay `new Student()` → `StudentCreateDto` |
| 4.2 | `UsersController.cs` | 🔴 | Thay `new User()`, `new Student()` → DTOs |
| 4.3 | `ClassesController.cs` | 🔴 | Thay `new Enrollment()` → `EnrollmentCreateDto` |
| 4.4 | `EnrollmentsController.cs` | 🟡 | Thay `new Enrollment()` → DTO |
| 4.5 | `ClassAssignmentController.cs` | 🟡 | Thay `new Enrollment()` → DTO |
| 4.6 | `SeedController.cs` | 🟢 | Giữ nguyên hoặc chuyển sang DbInitializer |
| 4.7 | `CoursesController.cs` | ✅ | Đã refactor |
| 4.8 | `SemestersController.cs` | ✅ | Đã refactor |
| 4.9 | `NotificationsController.cs` | ✅ | Đã refactor |
| 4.10 | `GradesController.cs` | ✅ | Đã refactor |

### PHASE 5: Cleanup GlobalUsings
> Xóa bypass DataAccess trong MVC

| # | File | Công việc |
|---|------|-----------|
| 5.1 | `StudentManagementMVC/GlobalUsings.cs` | Chỉ import Services.Models |
| 5.2 | `Services/Models/Enums.cs` | Re-export enums từ DataAccess |
| 5.3 | `Services/Helpers/EnumConverter.cs` | Xóa nếu dùng chung enums |

### PHASE 6: Update Views
> Đảm bảo Views dùng DTOs

| # | Folder | Ghi chú |
|---|--------|---------|
| 6.1 | `Views/Students/` | Kiểm tra @model |
| 6.2 | `Views/Users/` | Kiểm tra @model |
| 6.3 | `Views/Classes/` | Kiểm tra @model |
| 6.4 | `Views/Semesters/` | Kiểm tra @model |
| 6.5 | `Views/Courses/` | Kiểm tra @model |

---

## 📌 THỨ TỰ THỰC HIỆN

### 🚀 Bước 1: Fix lỗi build hiện tại
1. Sửa `Services/Models/Enums.cs` - re-export enums từ DataAccess (không duplicate)
2. Cập nhật `GlobalUsings.cs` trong MVC project
3. Fix ambiguous references

### 🚀 Bước 2: Hoàn thiện DTOs
1. Tạo `RoleDto.cs`
2. Tạo `TeacherDto.cs`  
3. Thêm `UserUpdateDto` vào `UserDto.cs`

### 🚀 Bước 3: Cập nhật Service Interfaces & Implementations
1. Thêm DTO-based methods vào interfaces
2. Implement mapping trong services

### 🚀 Bước 4: Refactor Controllers (theo thứ tự ưu tiên)
1. StudentsController
2. UsersController
3. ClassesController
4. EnrollmentsController
5. ClassAssignmentController

### 🚀 Bước 5: Kiểm tra & Test
1. Build solution
2. Chạy ứng dụng
3. Test các chức năng chính

---

## ⚠️ LƯU Ý QUAN TRỌNG

1. **SeedController**: Có thể giữ nguyên vì đây là tool seed data, không phải business logic
2. **Enums**: Định nghĩa ở DataAccess, re-export qua Services để MVC dùng được
3. **Navigation Properties**: Services cần map navigation properties sang nested DTOs
4. **Backward Compatibility**: Giữ cả Entity methods và DTO methods trong Services để transition dần

---

## 📈 TIẾN ĐỘ

- [x] Phân tích vi phạm
- [x] Tạo kế hoạch
- [ ] Phase 1: DTOs
- [ ] Phase 2: Service Interfaces
- [ ] Phase 3: Service Implementations
- [ ] Phase 4: Controllers
- [ ] Phase 5: Cleanup
- [ ] Phase 6: Views

---

*Cập nhật: 30/01/2026*
