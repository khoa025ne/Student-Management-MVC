using DataAccess.Entities;
using DataAccess.DAO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager,Teacher")]
    public class EnrollmentGradesController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IClassService _classService;
        private readonly ISemesterService _semesterService;
        private readonly IGeminiAIService _geminiAIService;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IAcademicAnalysisRepository _academicAnalysisRepository;

        public EnrollmentGradesController(
            IEnrollmentService enrollmentService,
            IClassService classService,
            ISemesterService semesterService,
            IGeminiAIService geminiAIService,
            IEmailService emailService,
            INotificationService notificationService,
            IAcademicAnalysisRepository academicAnalysisRepository)
        {
            _enrollmentService = enrollmentService;
            _classService = classService;
            _semesterService = semesterService;
            _geminiAIService = geminiAIService;
            _emailService = emailService;
            _notificationService = notificationService;
            _academicAnalysisRepository = academicAnalysisRepository;
        }

        // GET: EnrollmentGrades
        public async Task<IActionResult> Index()
        {
            var semesters = await _semesterService.GetAllAsync();
            ViewData["Semesters"] = new SelectList(semesters, "SemesterId", "SemesterName");
            return View();
        }

        // GET: EnrollmentGrades/SelectClass?semesterId=1
        public async Task<IActionResult> SelectClass(int semesterId)
        {
            var classes = await _classService.GetBySemesterAsync(semesterId);
            var semester = await _semesterService.GetByIdAsync(semesterId);
            
            ViewBag.SemesterName = semester?.SemesterName;
            ViewBag.SemesterId = semesterId;
            
            return View(classes);
        }

        // GET: EnrollmentGrades/ClassDetails/5
        public async Task<IActionResult> ClassDetails(int classId)
        {
            var enrollments = await _enrollmentService.GetByClassAsync(classId);
            var classEntity = await _classService.GetByIdAsync(classId);
            
            if (classEntity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lớp học!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClassName = classEntity.ClassName;
            ViewBag.ClassId = classId;
            
            return View(enrollments);
        }

        // GET: EnrollmentGrades/Edit/5
        public async Task<IActionResult> Edit(int enrollmentId)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId);
            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đăng ký!";
                return RedirectToAction(nameof(Index));
            }

            return View(enrollment);
        }

        // POST: EnrollmentGrades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int enrollmentId, double? midtermScore, double? finalScore)
        {
            try
            {
                // Validate scores
                if (midtermScore.HasValue && (midtermScore.Value < 0 || midtermScore.Value > 10))
                {
                    TempData["ErrorMessage"] = "Điểm giữa kỳ phải trong khoảng 0-10!";
                    return RedirectToAction(nameof(Edit), new { enrollmentId });
                }

                if (finalScore.HasValue && (finalScore.Value < 0 || finalScore.Value > 10))
                {
                    TempData["ErrorMessage"] = "Điểm cuối kỳ phải trong khoảng 0-10!";
                    return RedirectToAction(nameof(Edit), new { enrollmentId });
                }

                // Update grades using EnrollmentService (tự động tính TotalScore, Grade, IsPassed)
                var enrollment = await _enrollmentService.UpdateGradeAsync(enrollmentId, midtermScore, finalScore);

                // Reload để lấy thông tin đầy đủ với navigation properties
                enrollment = await _enrollmentService.GetByIdAsync(enrollmentId);

                if (enrollment != null && enrollment.Student?.User != null && enrollment.TotalScore.HasValue)
                {
                    var courseName = enrollment.Class?.Course?.CourseName ?? "N/A";
                    var grade = enrollment.Grade ?? "N/A";
                    var score = enrollment.TotalScore.Value;

                    // ═══════════════════════════════════════════════════════════════
                    // EVENT 1: SCORE UPDATE → Email + In-app Notification
                    // ═══════════════════════════════════════════════════════════════
                    await _notificationService.SendScoreUpdateNotificationAsync(
                        enrollment.StudentId,
                        courseName,
                        score,
                        grade
                    );

                    // ═══════════════════════════════════════════════════════════════
                    // EVENT 2: ACHIEVEMENT → Chúc mừng đạt A/A+
                    // ═══════════════════════════════════════════════════════════════
                    if (grade == "A+" || grade == "A")
                    {
                        await _notificationService.SendAchievementNotificationAsync(
                            enrollment.StudentId,
                            courseName,
                            grade
                        );
                    }

                    // ═══════════════════════════════════════════════════════════════
                    // EVENT 3: PERFORMANCE ALERT → Cảnh báo môn yếu D/F
                    // ═══════════════════════════════════════════════════════════════
                    if (grade == "D" || grade == "F")
                    {
                        var reason = grade == "F" 
                            ? "Môn học chưa đạt yêu cầu. Bạn cần học lại môn này."
                            : "Kết quả chưa tốt. Cần cải thiện để tránh ảnh hưởng đến GPA.";
                        
                        await _notificationService.SendPerformanceAlertNotificationAsync(
                            enrollment.StudentId,
                            courseName,
                            grade,
                            reason
                        );
                    }
                }

                // ═══════════════════════════════════════════════════════════════
                // FLOW 2: AI ANALYSIS + LEARNING PATH RECOMMENDATION
                // ═══════════════════════════════════════════════════════════════
                if (midtermScore.HasValue && finalScore.HasValue && enrollment != null)
                {
                    try
                    {
                        var aiResult = await _geminiAIService.AnalyzeStudentPerformanceAsync(enrollment.StudentId);
                        
                        if (aiResult.Success)
                        {
                            // Lưu AcademicAnalysis vào database
                            var academicAnalysis = new AcademicAnalysis
                            {
                                StudentId = enrollment.StudentId,
                                AnalysisDate = DateTime.Now,
                                OverallGPA = enrollment.Student?.OverallGPA ?? 0,
                                StrongSubjectsJson = JsonConvert.SerializeObject(aiResult.StrongSubjects),
                                WeakSubjectsJson = JsonConvert.SerializeObject(aiResult.WeakSubjects),
                                Recommendations = aiResult.Recommendations,
                                AiModelUsed = "Gemini-1.5-Pro"
                            };

                            await _academicAnalysisRepository.AddAsync(academicAnalysis);

                            // Gửi email AI analysis với insights chi tiết
                            if (enrollment.Student?.User != null)
                            {
                                await _emailService.SendAIAnalysisNotificationAsync(
                                    enrollment.Student.User.Email,
                                    enrollment.Student.User.FullName,
                                    academicAnalysis.StrongSubjectsJson,
                                    academicAnalysis.WeakSubjectsJson,
                                    academicAnalysis.Recommendations,
                                    academicAnalysis.OverallGPA
                                );
                            }

                            // ═══════════════════════════════════════════════════════════════
                            // EVENT 4: LEARNING PATH → Gợi ý môn học kỳ tới
                            // ═══════════════════════════════════════════════════════════════
                            var activeSemester = await _semesterService.GetActiveAsync();
                            if (activeSemester != null)
                            {
                                var learningPath = await _geminiAIService.GenerateLearningPathAsync(
                                    enrollment.StudentId, 
                                    activeSemester.SemesterId
                                );

                                if (learningPath.Success && learningPath.RecommendedCourses != null)
                                {
                                    var recommendedCourseNames = learningPath.RecommendedCourses
                                        .Select(c => c.CourseName)
                                        .ToArray();

                                    await _notificationService.SendLearningPathNotificationAsync(
                                        enrollment.StudentId,
                                        recommendedCourseNames
                                    );
                                }
                            }

                            TempData["SuccessMessage"] = "Cập nhật điểm thành công! AI đã phân tích và gửi gợi ý lộ trình học tập.";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Cập nhật điểm thành công! (AI analysis gặp lỗi: " + aiResult.ErrorMessage + ")";
                        }
                    }
                    catch (Exception aiEx)
                    {
                        // AI failed nhưng điểm vẫn lưu thành công
                        TempData["SuccessMessage"] = $"Cập nhật điểm thành công! (AI analysis lỗi: {aiEx.Message})";
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = "Cập nhật điểm thành công!";
                }

                // Quay về trang chi tiết lớp
                return RedirectToAction(nameof(ClassDetails), new { classId = enrollment.ClassId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật điểm: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { enrollmentId });
            }
        }

        // POST: EnrollmentGrades/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(int classId, int[] enrollmentIds, double?[] midtermScores, double?[] finalScores)
        {
            try
            {
                if (enrollmentIds == null || midtermScores == null || finalScores == null)
                {
                    TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";
                    return RedirectToAction(nameof(ClassDetails), new { classId });
                }

                if (enrollmentIds.Length != midtermScores.Length || enrollmentIds.Length != finalScores.Length)
                {
                    TempData["ErrorMessage"] = "Dữ liệu không đồng bộ!";
                    return RedirectToAction(nameof(ClassDetails), new { classId });
                }

                int successCount = 0;
                int errorCount = 0;

                for (int i = 0; i < enrollmentIds.Length; i++)
                {
                    try
                    {
                        var midterm = midtermScores[i];
                        var final = finalScores[i];

                        // Skip nếu không có điểm nào được nhập
                        if (!midterm.HasValue && !final.HasValue)
                            continue;

                        // Validate
                        if (midterm.HasValue && (midterm.Value < 0 || midterm.Value > 10))
                            continue;
                        if (final.HasValue && (final.Value < 0 || final.Value > 10))
                            continue;

                        // Update
                        await _enrollmentService.UpdateGradeAsync(enrollmentIds[i], midterm, final);
                        successCount++;

                        // TODO: Gửi email & AI analysis cho từng sinh viên
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                if (successCount > 0)
                {
                    TempData["SuccessMessage"] = $"Cập nhật thành công {successCount} điểm!";
                }
                if (errorCount > 0)
                {
                    TempData["ErrorMessage"] = $"Có {errorCount} điểm cập nhật thất bại!";
                }

                return RedirectToAction(nameof(ClassDetails), new { classId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(ClassDetails), new { classId });
            }
        }
    }
}
