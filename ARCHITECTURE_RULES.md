# 📐 Nguyên Tắc Kiến Trúc 3 Tầng - Student Management MVC

> **AI PHẢI TUÂN THỦ NGHIÊM NGẶT CÁC NGUYÊN TẮC SAU KHI THỰC HIỆN BẤT KỲ THAY ĐỔI NÀO**

## 🔄 Luồng Giao Tiếp Chuẩn

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Controllers (MVC)                           │   │
│  │  • Nhận HTTP Request                                     │   │
│  │  • Bind Model, Validate ModelState                       │   │
│  │  • Gọi Service tương ứng                                 │   │
│  │  • Trả View / JSON / Redirect                            │   │
│  └─────────────────────────────────────────────────────────┘   │
│                            │                                    │
│                            ▼                                    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │         Services.Interfaces + Services.Models            │   │
│  │  • IStudentService, ICourseService, etc.                 │   │
│  │  • DTOs, ViewModels                                      │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    BUSINESS/SERVICE LAYER                       │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Services.Implementations                    │   │
│  │  • Chứa nghiệp vụ (business logic)                       │   │
│  │  • Validate theo business rules                          │   │
│  │  • Xử lý transaction                                     │   │
│  │  • Mapping Entity ↔ DTO/ViewModel                        │   │
│  │  • Gọi Repository/DAO để CRUD                            │   │
│  └─────────────────────────────────────────────────────────┘   │
│                            │                                    │
│                            ▼                                    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Repositories.Interfaces                     │   │
│  │  • IStudentRepository, ICourseRepository, etc.           │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER                            │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │     Repositories.Implementations / DataAccess.DAO        │   │
│  │  • CRUD operations                                       │   │
│  │  • Database queries (LINQ/EF Core)                       │   │
│  │  • Không chứa business logic                             │   │
│  └─────────────────────────────────────────────────────────┘   │
│                            │                                    │
│                            ▼                                    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              DataAccess.AppDbContext                     │   │
│  │  • Entity Framework Core DbContext                       │   │
│  │  • DbSet<Entity>                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                            │                                    │
│                            ▼                                    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              DataAccess.Entities                         │   │
│  │  • Student, Course, Class, Enrollment, etc.              │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## ⛔ NGUYÊN TẮC BẮT BUỘC

### 1. Phụ Thuộc Giữa Các Tầng

| Tầng | Được phụ thuộc vào | KHÔNG được phụ thuộc vào |
|------|-------------------|-------------------------|
| **Presentation** | Services.Interfaces, Services.Models | DataAccess.*, Repositories.Implementations |
| **Service** | Repositories.Interfaces, DataAccess.Entities | Presentation |
| **DataAccess** | (Không phụ thuộc tầng khác) | Presentation, Services |

```
✅ ĐÚNG:   Presentation → Service → DataAccess
❌ SAI:    Presentation → DataAccess (đường tắt)
```

### 2. Using Statements Trong Controller

#### ✅ ĐƯỢC PHÉP:
```csharp
using Services.Interfaces;
using Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
```

#### ❌ KHÔNG ĐƯỢC PHÉP:
```csharp
using DataAccess;                    // ❌ VIOLATION
using DataAccess.DAO;                // ❌ VIOLATION
using DataAccess.Entities;           // ❌ VIOLATION
using Repositories.Implementations;  // ❌ VIOLATION
using Microsoft.EntityFrameworkCore; // ❌ VIOLATION (trong Controller)
```

### 3. Dependency Injection Trong Controller

#### ✅ ĐÚNG:
```csharp
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;

    public StudentController(
        IStudentService studentService,
        ICourseService courseService)
    {
        _studentService = studentService;
        _courseService = courseService;
    }
}
```

#### ❌ SAI:
```csharp
public class StudentController : Controller
{
    private readonly AppDbContext _context;        // ❌ VIOLATION
    private readonly IStudentRepository _repo;     // ❌ VIOLATION
    private readonly StudentDAO _dao;              // ❌ VIOLATION

    public StudentController(AppDbContext context) // ❌ VIOLATION
    {
        _context = context;
    }
}
```

### 4. Trách Nhiệm Của Mỗi Tầng

| Tầng | Trách nhiệm | KHÔNG làm |
|------|-------------|-----------|
| **Controller** | Nhận request, bind model, gọi service, trả response | Query database, business logic phức tạp |
| **Service** | Business logic, validation, mapping, transaction | Render view, xử lý HTTP |
| **Repository** | CRUD, database queries | Business rules, UI logic |

### 5. Mapping Entity ↔ DTO

- **Controller** chỉ làm việc với **DTO/ViewModel** từ Services.Models
- **Service** chịu trách nhiệm mapping **Entity ↔ DTO**
- **Controller KHÔNG** import Entity từ DataAccess.Entities

## 🔍 CHECKLIST KHI CODE REVIEW

### Cho mỗi Controller, kiểm tra:

- [ ] Không có `using DataAccess;` hoặc `using DataAccess.*;`
- [ ] Không có `using Repositories.*;`
- [ ] Không có `using Microsoft.EntityFrameworkCore;`
- [ ] Không inject `AppDbContext` trực tiếp
- [ ] Không inject Repository trực tiếp
- [ ] Chỉ inject các interface từ `Services.Interfaces`
- [ ] Không có LINQ query với DbSet trong Controller
- [ ] Không có `.Include()`, `.Where()`, `.ToListAsync()` trực tiếp trên Entity

### Cho mỗi Service, kiểm tra:

- [ ] Inject Repository/DAO qua interface
- [ ] Chứa business logic và validation
- [ ] Có mapping Entity ↔ DTO
- [ ] Không có code liên quan đến HTTP/View

## 📝 MẪU CÂU THUYẾT TRÌNH

> "Em áp dụng nguyên tắc: **Controller chỉ gọi Service, Service mới gọi DataAccess**, nên tầng Presentation không phụ thuộc trực tiếp vào DbContext hay Repository. Điều này đảm bảo **separation of concerns** và đúng mô hình **3-layer architecture**."

## 🔧 QUY TRÌNH SỬA LỖI

1. **Quét** tất cả Controllers để tìm vi phạm
2. **Tạo** Service interface + implementation nếu chưa có
3. **Di chuyển** logic truy cập database vào Service
4. **Cập nhật** Controller để gọi Service thay vì trực tiếp truy cập data
5. **Đăng ký** Service trong Program.cs
6. **Test** lại chức năng sau khi sửa

## ⚠️ LƯU Ý QUAN TRỌNG

- **LUÔN TEST** sau mỗi thay đổi
- Nếu cần Entity trong Controller (để hiển thị), phải tạo DTO tương ứng
- Mọi thay đổi liên quan đến database phải qua Service
- GlobalUsings.cs có thể gây nhầm lẫn - kiểm tra kỹ

---

**Cập nhật lần cuối:** 28/01/2026
**Áp dụng cho:** StudentManagementMVC Project
