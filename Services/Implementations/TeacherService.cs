using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations
{
    /// <summary>
    /// Teacher Service Implementation - Xử lý nghiệp vụ giáo viên
    /// TUÂN THỦ: Service được phép truy cập DataAccess
    /// </summary>
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherDto?> GetByIdAsync(int teacherId)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            return teacher != null ? MapToDto(teacher) : null;
        }

        public async Task<TeacherDto?> GetByUserIdAsync(int userId)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            return teacher != null ? MapToDto(teacher) : null;
        }

        public async Task<TeacherDashboardDto?> GetDashboardAsync(int teacherId)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher == null) return null;

            // Get current semester
            var currentSemester = await _context.Semesters
                .Where(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .FirstOrDefaultAsync();

            // Get classes assigned to this teacher this semester
            var myClasses = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Where(c => c.TeacherId == teacherId &&
                           (currentSemester == null || c.SemesterId == currentSemester.SemesterId))
                .ToListAsync();

            // Get today's schedule
            var today = DateTime.Now.DayOfWeek;
            var todaySchedule = myClasses
                .Where(c => IsClassToday(c.Schedule, today))
                .OrderBy(c => GetTimeSlotOrder(c.TimeSlot.ToString()))
                .Select(c => new TodayScheduleDto
                {
                    Id = c.ClassId,
                    ClassCode = c.ClassCode ?? "",
                    CourseName = c.Course?.CourseName ?? "N/A",
                    Room = c.Room ?? "",
                    TimeSlot = c.TimeSlot.ToString(),
                    StudentCount = c.Enrollments.Count,
                    MaxCapacity = c.MaxCapacity
                })
                .ToList();

            // Calculate statistics
            var totalStudents = myClasses.SelectMany(c => c.Enrollments).Select(e => e.StudentId).Distinct().Count();
            var totalClasses = myClasses.Count;

            // Get grades distribution
            var gradeDistribution = await GetGradeDistributionAsync(teacherId);

            // Students needing attention
            var studentsNeedingAttention = await GetStudentsNeedingAttentionAsync(teacherId);

            // Recent score entries
            var recentScoreEntries = await GetRecentScoreEntriesAsync(teacherId);

            // Unread notifications
            var unreadNotifications = await GetUnreadNotificationCountAsync(teacherId);

            // Classes without scores
            var classIds = myClasses.Select(c => c.ClassId).ToList();
            var classesWithScores = await _context.Scores
                .Where(s => s.ClassId.HasValue && classIds.Contains(s.ClassId.Value))
                .Select(s => s.ClassId!.Value)
                .Distinct()
                .ToListAsync();

            var classesWithoutScores = myClasses
                .Where(c => c.Enrollments.Any() && !classesWithScores.Contains(c.ClassId))
                .Select(c => new ClassWithoutScoreDto
                {
                    Id = c.ClassId,
                    ClassCode = c.ClassCode ?? "",
                    CourseName = c.Course?.CourseName ?? "N/A",
                    StudentCount = c.Enrollments.Count
                })
                .ToList();

            return new TeacherDashboardDto
            {
                Teacher = MapToDto(teacher),
                CurrentSemester = currentSemester != null ? new SemesterDto
                {
                    SemesterId = currentSemester.SemesterId,
                    SemesterName = currentSemester.SemesterName ?? "",
                    StartDate = currentSemester.StartDate,
                    EndDate = currentSemester.EndDate
                } : null,
                TotalClasses = totalClasses,
                TotalStudents = totalStudents,
                TodaySchedule = todaySchedule,
                MyClasses = myClasses.Select(c => new TeacherClassDto
                {
                    Id = c.ClassId,
                    ClassCode = c.ClassCode ?? "",
                    ClassName = c.ClassName ?? "",
                    CourseName = c.Course?.CourseName ?? "N/A",
                    CourseCode = c.Course?.CourseCode ?? "",
                    Room = c.Room ?? "",
                    TimeSlot = c.TimeSlot.ToString(),
                    Schedule = c.Schedule ?? "",
                    StudentCount = c.Enrollments.Count,
                    MaxCapacity = c.MaxCapacity
                }).ToList(),
                GradeDistribution = gradeDistribution,
                StudentsNeedingAttention = studentsNeedingAttention.ToList(),
                RecentScoreEntries = recentScoreEntries.ToList(),
                UnreadNotifications = unreadNotifications,
                ClassesWithoutScores = classesWithoutScores
            };
        }

        public async Task<IEnumerable<TeacherClassDto>> GetMyClassesAsync(int teacherId)
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.Semester!.StartDate)
                .ToListAsync();

            return classes.Select(c => new TeacherClassDto
            {
                Id = c.ClassId,
                ClassCode = c.ClassCode ?? "",
                ClassName = c.ClassName ?? "",
                CourseName = c.Course?.CourseName ?? "N/A",
                CourseCode = c.Course?.CourseCode ?? "",
                Room = c.Room ?? "",
                TimeSlot = c.TimeSlot.ToString(),
                Schedule = c.Schedule ?? "",
                StudentCount = c.Enrollments.Count,
                MaxCapacity = c.MaxCapacity,
                SemesterName = c.Semester?.SemesterName ?? "",
                SemesterStartDate = c.Semester?.StartDate
            });
        }

        public async Task<TeacherClassDetailDto?> GetClassDetailAsync(int classId)
        {
            var cls = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Scores)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (cls == null) return null;

            return new TeacherClassDetailDto
            {
                ClassId = cls.ClassId,
                ClassCode = cls.ClassCode ?? "",
                ClassName = cls.ClassName ?? "",
                CourseName = cls.Course?.CourseName ?? "N/A",
                CourseCode = cls.Course?.CourseCode ?? "",
                Credits = cls.Course?.Credits ?? 0,
                Room = cls.Room ?? "",
                TimeSlot = cls.TimeSlot.ToString(),
                Schedule = cls.Schedule ?? "",
                SemesterName = cls.Semester?.SemesterName ?? "",
                Students = cls.Enrollments.Select(e =>
                {
                    var score = e.Scores.FirstOrDefault(s => s.ClassId == classId);
                    return new StudentEnrollmentDto
                    {
                        StudentId = e.StudentId,
                        StudentCode = e.Student?.StudentCode ?? "",
                        FullName = e.Student?.FullName ?? "",
                        Email = e.Student?.Email ?? "",
                        AttendanceScore = score?.AttendanceScore,
                        Assignment1Score = score?.Assignment1Score,
                        Assignment2Score = score?.Assignment2Score,
                        MidtermScore = score?.MidtermScore,
                        FinalExamScore = score?.FinalExamScore,
                        TotalScore = score?.TotalScore,
                        Grade = score?.Grade
                    };
                }).ToList()
            };
        }

        public async Task<TeacherClassDetailDto?> GetClassForGradeEntryAsync(int classId)
        {
            return await GetClassDetailAsync(classId);
        }

        public async Task<bool> SaveGradesAsync(int classId, List<ScoreEntryDto> scores)
        {
            try
            {
                foreach (var entry in scores)
                {
                    var existingScore = await _context.Scores
                        .FirstOrDefaultAsync(s => s.StudentId == entry.StudentId && s.ClassId == classId);

                    if (existingScore != null)
                    {
                        existingScore.AttendanceScore = entry.AttendanceScore;
                        existingScore.Assignment1Score = entry.Assignment1Score;
                        existingScore.Assignment2Score = entry.Assignment2Score;
                        existingScore.MidtermScore = entry.MidtermScore;
                        existingScore.FinalExamScore = entry.FinalExamScore;
                        existingScore.ModifiedDate = DateTime.Now;
                        existingScore.CalculateTotalScore();
                    }
                    else
                    {
                        var newScore = new Score
                        {
                            StudentId = entry.StudentId,
                            ClassId = classId,
                            AttendanceScore = entry.AttendanceScore,
                            Assignment1Score = entry.Assignment1Score,
                            Assignment2Score = entry.Assignment2Score,
                            MidtermScore = entry.MidtermScore,
                            FinalExamScore = entry.FinalExamScore,
                            CreatedDate = DateTime.Now
                        };
                        newScore.CalculateTotalScore();
                        _context.Scores.Add(newScore);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<RecentScoreEntryDto>> GetRecentScoreEntriesAsync(int teacherId, int count = 5)
        {
            var myClassIds = await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ClassId)
                .ToListAsync();

            var recentScores = await _context.Scores
                .Include(s => s.Student)
                .Include(s => s.Class)
                    .ThenInclude(c => c!.Course)
                .Where(s => s.ClassId.HasValue && myClassIds.Contains(s.ClassId.Value))
                .OrderByDescending(s => s.ModifiedDate ?? s.CreatedDate)
                .Take(count)
                .ToListAsync();

            return recentScores.Select(s => new RecentScoreEntryDto
            {
                StudentCode = s.Student?.StudentCode ?? "",
                StudentName = s.Student?.FullName ?? "",
                CourseName = s.Class?.Course?.CourseName ?? "",
                ScoreType = GetScoreType(s),
                TotalScore = s.TotalScore,
                EntryDate = s.ModifiedDate ?? s.CreatedDate ?? DateTime.Now
            });
        }

        public async Task<GradeDistributionDto> GetGradeDistributionAsync(int teacherId)
        {
            var myClassIds = await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ClassId)
                .ToListAsync();

            var studentIds = await _context.Enrollments
                .Where(e => myClassIds.Contains(e.ClassId))
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync();

            var grades = await _context.Scores
                .Where(s => studentIds.Contains(s.StudentId) && s.TotalScore.HasValue)
                .Select(s => s.TotalScore)
                .ToListAsync();

            int gradeA = 0, gradeB = 0, gradeC = 0, gradeD = 0, gradeF = 0;
            foreach (var score in grades)
            {
                if (!score.HasValue) continue;
                var val = score.Value;
                if (val >= 8.5) gradeA++;
                else if (val >= 7.0) gradeB++;
                else if (val >= 5.5) gradeC++;
                else if (val >= 4.0) gradeD++;
                else gradeF++;
            }

            return new GradeDistributionDto
            {
                GradeACount = gradeA,
                GradeBCount = gradeB,
                GradeCCount = gradeC,
                GradeDCount = gradeD,
                GradeFCount = gradeF
            };
        }

        public async Task<IEnumerable<StudentAlertDto>> GetStudentsNeedingAttentionAsync(int teacherId, int count = 5)
        {
            var myClassIds = await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ClassId)
                .ToListAsync();

            var alertStudents = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                .Include(e => e.Scores)
                .Where(e => myClassIds.Contains(e.ClassId))
                .Where(e => e.Scores.Any(s => s.TotalScore < 4.0))
                .Take(count)
                .ToListAsync();

            return alertStudents.Select(e =>
            {
                var latestScore = e.Scores
                    .Where(s => s.TotalScore < 4.0)
                    .OrderByDescending(s => s.ModifiedDate ?? s.CreatedDate)
                    .FirstOrDefault();

                return new StudentAlertDto
                {
                    StudentCode = e.Student?.StudentCode ?? "",
                    StudentName = e.Student?.FullName ?? "",
                    CourseName = e.Class?.Course?.CourseName ?? "",
                    LatestScore = latestScore != null ? new StudentAlertScoreDto { TotalScore = latestScore.TotalScore } : null,
                    ScoreDate = latestScore?.ModifiedDate ?? latestScore?.CreatedDate
                };
            });
        }

        public async Task<int> GetUnreadNotificationCountAsync(int teacherId)
        {
            return await _context.Notifications
                .Where(n => n.TeacherId == teacherId && !n.IsRead)
                .CountAsync();
        }

        #region Helper Methods

        private static TeacherDto MapToDto(Teacher teacher)
        {
            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                TeacherCode = teacher.TeacherCode ?? "",
                FullName = teacher.FullName ?? "",
                Email = teacher.Email ?? "",
                Phone = teacher.PhoneNumber,
                Department = teacher.Department,
                Specialization = teacher.Specialization,
                HireDate = teacher.CreatedAt
            };
        }

        private static bool IsClassToday(string? schedule, DayOfWeek today)
        {
            if (string.IsNullOrEmpty(schedule)) return false;

            var dayMapping = new Dictionary<string, DayOfWeek>
            {
                {"T2", DayOfWeek.Monday},
                {"T3", DayOfWeek.Tuesday},
                {"T4", DayOfWeek.Wednesday},
                {"T5", DayOfWeek.Thursday},
                {"T6", DayOfWeek.Friday},
                {"T7", DayOfWeek.Saturday},
                {"CN", DayOfWeek.Sunday}
            };

            return dayMapping.Any(kv => schedule.Contains(kv.Key) && kv.Value == today);
        }

        private static int GetTimeSlotOrder(string? timeSlot)
        {
            if (string.IsNullOrEmpty(timeSlot)) return 99;
            var parts = timeSlot.Split('-');
            if (int.TryParse(parts[0], out int slot))
            {
                return slot;
            }
            return 99;
        }

        private static string GetScoreType(Score score)
        {
            if (score.FinalExamScore.HasValue) return "Cuối kỳ";
            if (score.MidtermScore.HasValue) return "Giữa kỳ";
            if (score.Assignment2Score.HasValue) return "Bài tập 2";
            if (score.Assignment1Score.HasValue) return "Bài tập 1";
            if (score.AttendanceScore.HasValue) return "Chuyên cần";
            return "Điểm";
        }

        #endregion
    }
}
