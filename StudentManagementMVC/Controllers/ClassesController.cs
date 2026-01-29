using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using DataAccess.Entities;

namespace StudentManagementMVC.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;

        public ClassesController(
            IClassService classService, 
            ICourseService courseService, 
            ISemesterService semesterService,
            IStudentService studentService,
            IEnrollmentService enrollmentService)
        {
            _classService = classService;
            _courseService = courseService;
            _semesterService = semesterService;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
        }

        // Admin, Manager: Manage all classes
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 9)
        {
            var allClasses = await _classService.GetAllAsync();
            var totalItems = allClasses.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Đảm bảo page hợp lệ
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
            
            var classes = allClasses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            
            return View(classes);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Class classEntity)
        {
            // Remove validation errors for navigation properties
            ModelState.Remove("Course");
            ModelState.Remove("Semester");
            ModelState.Remove("Enrollments");
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra duplicate ClassCode
                    var existingClasses = await _classService.GetAllAsync();
                    if (existingClasses.Any(c => c.ClassCode == classEntity.ClassCode))
                    {
                        TempData["ErrorMessage"] = $"Mã lớp '{classEntity.ClassCode}' đã tồn tại! Vui lòng sử dụng mã lớp khác.";
                        ViewBag.Courses = await _courseService.GetAllAsync();
                        ViewBag.Semesters = await _semesterService.GetAllAsync();
                        return View(classEntity);
                    }

                    // Sync MaxStudents with MaxCapacity
                    if (classEntity.MaxCapacity > 0)
                    {
                        classEntity.MaxStudents = classEntity.MaxCapacity;
                    }
                    else if (classEntity.MaxStudents > 0)
                    {
                        classEntity.MaxCapacity = classEntity.MaxStudents;
                    }
                    else
                    {
                        classEntity.MaxCapacity = 30;
                        classEntity.MaxStudents = 30;
                    }

                    // Set default value for Schedule if null (legacy field)
                    if (string.IsNullOrEmpty(classEntity.Schedule))
                    {
                        classEntity.Schedule = "";
                    }

                    await _classService.CreateClassAsync(classEntity);
                    TempData["SuccessMessage"] = $"Tạo lớp học '{classEntity.ClassCode}' thành công! Sĩ số tối đa: {classEntity.MaxStudents} sinh viên.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tạo lớp học: {ex.Message}";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                // Log validation errors for debugging
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Dữ liệu không hợp lệ! Chi tiết: {errors}";
            }
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var classEntity = await _classService.GetByIdAsync(id);
            if (classEntity == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy lớp học với ID: {id}!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(Class classEntity)
        {
            // Remove validation errors for navigation properties
            ModelState.Remove("Course");
            ModelState.Remove("Semester");
            ModelState.Remove("Enrollments");
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra lớp học tồn tại
                    var existingClass = await _classService.GetByIdAsync(classEntity.ClassId);
                    if (existingClass == null)
                    {
                        TempData["ErrorMessage"] = $"Không tìm thấy lớp học với ID: {classEntity.ClassId}!";
                        return RedirectToAction(nameof(Index));
                    }

                    // Kiểm tra duplicate ClassCode (trừ chính nó)
                    var allClasses = await _classService.GetAllAsync();
                    if (allClasses.Any(c => c.ClassCode == classEntity.ClassCode && c.ClassId != classEntity.ClassId))
                    {
                        TempData["ErrorMessage"] = $"Mã lớp '{classEntity.ClassCode}' đã được sử dụng bởi lớp khác!";
                        ViewBag.Courses = await _courseService.GetAllAsync();
                        ViewBag.Semesters = await _semesterService.GetAllAsync();
                        return View(classEntity);
                    }

                    // Kiểm tra sĩ số
                    if (classEntity.MaxStudents <= 0 && classEntity.MaxCapacity <= 0)
                    {
                        TempData["ErrorMessage"] = "Sĩ số tối đa phải lớn hơn 0!";
                        ViewBag.Courses = await _courseService.GetAllAsync();
                        ViewBag.Semesters = await _semesterService.GetAllAsync();
                        return View(classEntity);
                    }

                    // Sync MaxStudents with MaxCapacity
                    if (classEntity.MaxCapacity > 0)
                    {
                        classEntity.MaxStudents = classEntity.MaxCapacity;
                    }

                    // Set default value for Schedule if null (legacy field)
                    if (string.IsNullOrEmpty(classEntity.Schedule))
                    {
                        classEntity.Schedule = existingClass.Schedule ?? "";
                    }

                    // Nếu không cập nhật lịch học/giờ học, giữ nguyên giá trị cũ
                    // Note: DayOfWeekPair and TimeSlot are enums, check if they're at default values
                    if (classEntity.DayOfWeekPair == 0)
                    {
                        classEntity.DayOfWeekPair = existingClass.DayOfWeekPair;
                    }
                    if (classEntity.TimeSlot == 0)
                    {
                        classEntity.TimeSlot = existingClass.TimeSlot;
                    }

                    await _classService.UpdateClassAsync(classEntity);
                    TempData["SuccessMessage"] = $"Cập nhật lớp học '{classEntity.ClassCode}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật lớp học: {ex.Message}";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                // Log validation errors for debugging
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Dữ liệu không hợp lệ! Chi tiết: {errors}";
            }
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Semesters = await _semesterService.GetAllAsync();
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Kiểm tra lớp học tồn tại
                var classEntity = await _classService.GetByIdAsync(id);
                if (classEntity == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy lớp học với ID: {id}!";
                    return RedirectToAction(nameof(Index));
                }

                var className = classEntity.ClassCode;
                await _classService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Xóa lớp học '{className}' thành công!";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY") || ex.Message.Contains("foreign key"))
                {
                    TempData["ErrorMessage"] = $"Không thể xóa lớp học: {ex.Message}. Có thể lớp học đang có sinh viên đăng ký hoặc điểm số!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa lớp học: {ex.Message}";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Admin, Manager, Teacher: View class details with student list and scores
        [Authorize(Roles = "Admin,Manager,Teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var classEntity = await _classService.GetByIdAsync(id);
            if (classEntity == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy lớp học với ID: {id}!";
                return RedirectToAction(nameof(Index));
            }

            // Get all enrollments for this class with student information
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var classEnrollments = allEnrollments
                .Where(e => e.ClassId == id)
                .OrderBy(e => e.Student?.StudentCode)
                .ToList();

            ViewBag.Enrollments = classEnrollments;
            ViewBag.TotalStudents = classEnrollments.Count();
            ViewBag.PassedStudents = classEnrollments.Count(e => e.IsPassed == true);
            ViewBag.AverageScore = classEnrollments
                .Where(e => e.TotalScore.HasValue)
                .Select(e => e.TotalScore!.Value)
                .DefaultIfEmpty(0)
                .Average();

            return View(classEntity);
        }

        // Teacher: View their classes
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> MyClasses(int page = 1, int pageSize = 9)
        {
            try
            {
                var allClasses = await _classService.GetAllAsync();
                
                // Filter classes assigned to this teacher
                // Sẽ cần thêm TeacherId trong Class entity để properly filter
                // Tạm thời hiển thị tất cả classes với message
                var totalItems = allClasses.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                
                page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
                
                var classes = allClasses
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = totalItems;
                ViewBag.IsTeacher = true;
                
                return View("Index", classes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách lớp giảng dạy: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // Random add students to a class
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RandomAddStudents(int classId, int count = 5)
        {
            try
            {
                // Kiểm tra lớp học tồn tại
                var classEntity = await _classService.GetByIdAsync(classId);
                if (classEntity == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học!";
                    return RedirectToAction(nameof(Details), new { id = classId });
                }

                // Kiểm tra sĩ số còn trống
                var currentEnrollments = await _enrollmentService.GetByClassAsync(classId);
                var availableSlots = classEntity.MaxStudents - currentEnrollments.Count();

                if (availableSlots <= 0)
                {
                    TempData["ErrorMessage"] = "Lớp học đã đầy! Không thể thêm sinh viên.";
                    return RedirectToAction(nameof(Details), new { id = classId });
                }

                // Giới hạn số lượng thêm
                count = Math.Min(count, availableSlots);

                // Lấy danh sách tất cả sinh viên
                var allStudents = await _studentService.GetAllStudentsAsync();
                
                // Lọc sinh viên chưa đăng ký lớp này
                var enrolledStudentIds = currentEnrollments.Select(e => e.StudentId).ToHashSet();
                var availableStudents = allStudents
                    .Where(s => !enrolledStudentIds.Contains(s.StudentId))
                    .ToList();

                if (!availableStudents.Any())
                {
                    TempData["ErrorMessage"] = "Không còn sinh viên nào để thêm vào lớp!";
                    return RedirectToAction(nameof(Details), new { id = classId });
                }

                // Random chọn sinh viên
                var random = new Random();
                var selectedStudents = availableStudents
                    .OrderBy(x => random.Next())
                    .Take(count)
                    .ToList();

                // Thêm vào lớp
                int successCount = 0;
                var errors = new List<string>();
                
                foreach (var student in selectedStudents)
                {
                    try
                    {
                        var enrollment = new Enrollment
                        {
                            StudentId = student.StudentId,
                            ClassId = classId,
                            EnrollmentDate = DateTime.Now,
                            Status = "Active",
                            AttemptNumber = 1
                        };

                        await _enrollmentService.CreateAsync(enrollment);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"SV {student.StudentCode}: {ex.Message}");
                    }
                }

                if (successCount > 0)
                {
                    TempData["SuccessMessage"] = $"Đã thêm {successCount}/{selectedStudents.Count} sinh viên vào lớp '{classEntity.ClassName}' thành công!";
                    if (errors.Any())
                    {
                        TempData["WarningMessage"] = $"Một số sinh viên không thêm được: {string.Join("; ", errors.Take(3))}";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Không thể thêm sinh viên vào lớp! Chi tiết: {string.Join("; ", errors)}";
                }

                return RedirectToAction(nameof(Details), new { id = classId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = classId });
            }
        }

        // View to select students to add
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddStudents(int classId)
        {
            try
            {
                var classEntity = await _classService.GetByIdAsync(classId);
                if (classEntity == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học!";
                    return RedirectToAction(nameof(Index));
                }

                // Get current enrollments
                var currentEnrollments = await _enrollmentService.GetByClassAsync(classId);
                var enrolledStudentIds = currentEnrollments.Select(e => e.StudentId).ToHashSet();

                // Get available students (using Entity-based method)
                var allStudents = await _studentService.GetAllStudentsAsync();
                var availableStudents = allStudents
                    .Where(s => !enrolledStudentIds.Contains(s.StudentId))
                    .OrderBy(s => s.StudentCode)
                    .ToList();

                ViewBag.Class = classEntity;
                ViewBag.AvailableSlots = classEntity.MaxStudents - currentEnrollments.Count();
                ViewBag.CurrentEnrollment = currentEnrollments.Count();

                return View(availableStudents);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Add selected students to class
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddSelectedStudents(int classId, int[] studentIds)
        {
            try
            {
                if (studentIds == null || studentIds.Length == 0)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sinh viên!";
                    return RedirectToAction(nameof(AddStudents), new { classId });
                }

                var classEntity = await _classService.GetByIdAsync(classId);
                if (classEntity == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học!";
                    return RedirectToAction(nameof(Index));
                }

                // Check available slots
                var currentEnrollments = await _enrollmentService.GetByClassAsync(classId);
                var availableSlots = classEntity.MaxStudents - currentEnrollments.Count();

                if (studentIds.Length > availableSlots)
                {
                    TempData["ErrorMessage"] = $"Chỉ còn {availableSlots} chỗ trống! Bạn đã chọn {studentIds.Length} sinh viên.";
                    return RedirectToAction(nameof(AddStudents), new { classId });
                }

                // Add students
                int successCount = 0;
                var errors = new List<string>();
                var skippedCount = 0;
                
                foreach (var studentId in studentIds)
                {
                    try
                    {
                        // Check if already enrolled
                        if (await _enrollmentService.IsEnrolledAsync(studentId, classId))
                        {
                            skippedCount++;
                            continue;
                        }

                        var enrollment = new Enrollment
                        {
                            StudentId = studentId,
                            ClassId = classId,
                            EnrollmentDate = DateTime.Now,
                            Status = "Active",
                            AttemptNumber = 1
                        };

                        await _enrollmentService.CreateAsync(enrollment);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        var student = await _studentService.GetByIdAsync(studentId);
                        var studentCode = student?.StudentCode ?? studentId.ToString();
                        errors.Add($"SV {studentCode}: {ex.Message}");
                    }
                }

                if (successCount > 0)
                {
                    var message = $"Đã thêm {successCount}/{studentIds.Length} sinh viên vào lớp '{classEntity.ClassName}' thành công!";
                    if (skippedCount > 0)
                    {
                        message += $" ({skippedCount} sinh viên đã đăng ký trước đó)";
                    }
                    TempData["SuccessMessage"] = message;
                    
                    if (errors.Any())
                    {
                        TempData["WarningMessage"] = $"Lỗi khi thêm: {string.Join("; ", errors.Take(5))}";
                    }
                }
                else
                {
                    if (skippedCount == studentIds.Length)
                    {
                        TempData["ErrorMessage"] = "Tất cả sinh viên đã được đăng ký vào lớp này rồi!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Không thể thêm sinh viên vào lớp! Chi tiết: {string.Join("; ", errors)}";
                    }
                }

                return RedirectToAction(nameof(Details), new { id = classId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = classId });
            }
        }

        // API endpoint để lấy danh sách lớp theo môn học
        [HttpGet]
        [Route("api/classes/by-course/{courseId}")]
        public async Task<IActionResult> GetClassesByCourse(int courseId)
        {
            try
            {
                var allClasses = await _classService.GetAllAsync();
                var classes = allClasses
                    .Where(c => c.CourseId == courseId)
                    .Select(c => new
                    {
                        classId = c.ClassId,
                        className = c.ClassName,
                        classCode = c.ClassCode,
                        courseId = c.CourseId,
                        courseName = c.Course?.CourseName,
                        courseCode = c.Course?.CourseCode,
                        room = c.Room,
                        dayOfWeek = c.DayOfWeekPair.ToString(),
                        timeSlot = c.TimeSlot.ToString(),
                        currentEnrollment = c.CurrentEnrollment,
                        maxCapacity = c.MaxCapacity
                    })
                    .ToList();

                return Json(classes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
