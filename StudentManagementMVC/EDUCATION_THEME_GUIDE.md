# Hướng dẫn sử dụng Education Theme

## Giới thiệu

Education Theme là hệ thống UI/UX mới được thiết kế theo template "Education Website" với phong cách hiện đại, tối giản và chuyên nghiệp cho hệ thống quản lý sinh viên.

## Cấu trúc files

### CSS Theme
```
wwwroot/css/education-theme.css   - File CSS chính chứa toàn bộ styles
```

### Layouts
```
Views/Shared/
├── _Layout.Education.cshtml      - Layout cho Student & Teacher portal
├── _Layout.AdminDark.cshtml      - Layout dark mode cho Admin/Manager
└── _EducationComponents.cshtml   - Partial view chứa các component tái sử dụng
```

### Views với Education Theme
```
Views/
├── Student/
│   └── Dashboard.Education.cshtml     - Dashboard sinh viên
├── Teacher/
│   └── Dashboard.Education.cshtml     - Dashboard giảng viên
├── Dashboard/
│   └── Index.Education.cshtml         - Dashboard Admin với thống kê
├── Courses/
│   └── Index.Education.cshtml         - Danh sách môn học
└── Students/
    └── Index.Education.cshtml         - Danh sách sinh viên
```

## Cách sử dụng

### 1. Chuyển đổi Layout

**Cho Student/Teacher views:**
```razor
@{
    Layout = "~/Views/Shared/_Layout.Education.cshtml";
}
```

**Cho Admin/Manager views:**
```razor
@{
    Layout = "~/Views/Shared/_Layout.AdminDark.cshtml";
}
```

### 2. Áp dụng cho tất cả views của một controller

Tạo file `_ViewStart.cshtml` trong thư mục views tương ứng:

**Views/Student/_ViewStart.cshtml:**
```razor
@{
    Layout = "~/Views/Shared/_Layout.Education.cshtml";
}
```

**Views/Dashboard/_ViewStart.cshtml:**
```razor
@{
    Layout = "~/Views/Shared/_Layout.AdminDark.cshtml";
}
```

### 3. Chuyển đổi động theo Role

Trong Controller, set ViewBag:
```csharp
public IActionResult Dashboard()
{
    var role = User.FindFirstValue(ClaimTypes.Role);
    ViewBag.Layout = role switch
    {
        "Student" or "Teacher" => "_Layout.Education",
        "Admin" or "Manager" => "_Layout.AdminDark",
        _ => "_Layout"
    };
    return View();
}
```

Trong View:
```razor
@{
    Layout = $"~/Views/Shared/{ViewBag.Layout}.cshtml";
}
```

## Bảng màu (Color Palette)

| Biến CSS | Màu | Sử dụng |
|----------|-----|---------|
| `--edu-primary` | #1751EF | Buttons, links, accent chính |
| `--edu-accent` | #A1543C | Secondary accent |
| `--edu-dark` | #0D0B0B | Background tối |
| `--edu-light` | #FBFBFB | Background sáng |
| `--edu-gray` | #A4A4A4 | Text phụ |
| `--edu-brown` | #544443 | Text accent |
| `--edu-success` | #22C55E | Thành công |
| `--edu-warning` | #F59E0B | Cảnh báo |
| `--edu-danger` | #EF4444 | Lỗi |
| `--edu-info` | #0EA5E9 | Thông tin |

## Các Component chính

### 1. Hero Section (Student/Teacher)
```html
<section class="edu-hero">
    <div class="edu-hero-content">
        <span class="edu-hero-badge">...</span>
        <h1>...</h1>
        <p>...</p>
        <div class="edu-hero-actions">
            <a class="edu-btn edu-btn-primary">...</a>
            <a class="edu-btn edu-btn-outline">...</a>
        </div>
    </div>
    <div class="edu-hero-cards">
        <div class="edu-floating-card">...</div>
    </div>
</section>
```

### 2. Stats Grid
```html
<div class="edu-stats-grid">
    <div class="edu-stat-card">
        <div class="edu-stat-icon primary">
            <i class="fas fa-..."></i>
        </div>
        <div class="edu-stat-info">
            <h3>100</h3>
            <p>Label</p>
        </div>
    </div>
</div>
```

### 3. Dark Card (Admin)
```html
<div class="edu-dark-card">
    <div class="edu-dark-card-header primary">
        <h5 class="edu-dark-card-title">
            <i class="fas fa-..."></i> Title
        </h5>
    </div>
    <div class="edu-dark-card-body">
        ...
    </div>
</div>
```

### 4. Buttons
```html
<a class="edu-btn edu-btn-primary">Primary</a>
<a class="edu-btn edu-btn-accent">Accent</a>
<a class="edu-btn edu-btn-success">Success</a>
<a class="edu-btn edu-btn-outline">Outline</a>
<a class="edu-btn edu-btn-sm">Small</a>
<a class="edu-btn edu-btn-lg">Large</a>
```

### 5. Tables
```html
<table class="edu-table">
    <thead>
        <tr><th>Header</th></tr>
    </thead>
    <tbody>
        <tr><td>Cell</td></tr>
    </tbody>
</table>
```

## Responsive Design

Theme hỗ trợ responsive với các breakpoints:
- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

## Typography

- **Display Font:** Plus Jakarta Sans (headings, titles)
- **Body Font:** Inter (paragraphs, content)

## Icons

Sử dụng Font Awesome 6.4.0:
```html
<i class="fas fa-icon-name"></i>
```

## Dark Mode (Admin)

Layout `_Layout.AdminDark.cshtml` mặc định sử dụng dark mode với:
- Background: #0D0B0B
- Cards: rgba(255, 255, 255, 0.03)
- Borders: rgba(255, 255, 255, 0.1)
- Text: white/rgba(255, 255, 255, 0.x)

## Animations

Theme bao gồm các animations:
- `fadeInUp`: Fade in từ dưới lên
- `float`: Float nhẹ lên xuống
- Hover transitions trên cards, buttons

## Lưu ý quan trọng

1. **Giữ nguyên backend**: Tất cả controller, model, route không thay đổi
2. **Files mới song song**: Các file .Education.cshtml được tạo song song với file gốc để dễ so sánh và rollback
3. **Testing**: Test kỹ trên các kích thước màn hình khác nhau
4. **SweetAlert2**: Sử dụng theme dark cho modals khi dùng layout dark

## Migration Guide

### Bước 1: Backup
Backup toàn bộ Views folder trước khi thay đổi

### Bước 2: Copy CSS
Đảm bảo `education-theme.css` được include trong layouts

### Bước 3: Thay đổi Views
- Rename các file gốc thành `.backup.cshtml`
- Rename các file `.Education.cshtml` thành file gốc

Hoặc:
- Cập nhật `_ViewStart.cshtml` để sử dụng layout mới

### Bước 4: Testing
Test toàn bộ chức năng trên tất cả roles (Student, Teacher, Admin, Manager)

## Hỗ trợ

Nếu gặp vấn đề với theme, kiểm tra:
1. CSS file được load đúng
2. Layout được set đúng
3. Font CDN accessible
4. Browser cache đã được clear
