using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DataEnums = DataAccess.Enums;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý thống kê Dashboard
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IClassRepository _classRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IScoreRepository _scoreRepo;

        public DashboardService(
            IStudentRepository studentRepo,
            IUserRepository userRepo,
            ICourseRepository courseRepo,
            IClassRepository classRepo,
            IEnrollmentRepository enrollmentRepo,
            IScoreRepository scoreRepo)
        {
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _courseRepo = courseRepo;
            _classRepo = classRepo;
            _enrollmentRepo = enrollmentRepo;
            _scoreRepo = scoreRepo;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
        {
            var stats = new DashboardStatistics();

            // Lấy tất cả dữ liệu cần thiết
            var students = await _studentRepo.GetAllAsync();
            var users = await _userRepo.GetAllAsync();
            var courses = await _courseRepo.GetAllAsync();
            var classes = await _classRepo.GetAllAsync();
            var enrollments = await _enrollmentRepo.GetAllAsync();
            var scores = await _scoreRepo.GetAllAsync();

            // Tổng quan cơ bản
            stats.TotalStudents = students.Count();
            stats.TotalCourses = courses.Count();
            stats.TotalClasses = classes.Count();
            stats.TotalEnrollments = enrollments.Count();
            stats.ActiveUsers = users.Count(u => u.IsActive);
            stats.TotalTeachers = users.Count(u => u.Role?.RoleName == "Staff" || u.Role?.RoleName == "Manager");

            // GPA trung bình
            var studentsWithGPA = students.Where(s => s.OverallGPA > 0);
            stats.AverageGPA = studentsWithGPA.Any() 
                ? Math.Round(studentsWithGPA.Average(s => s.OverallGPA), 2) 
                : 0;

            // Tỷ lệ lấp đầy lớp
            var classesWithCapacity = classes.Where(c => c.MaxStudents > 0);
            stats.ClassFillRate = classesWithCapacity.Any()
                ? Math.Round((double)classesWithCapacity.Sum(c => c.CurrentEnrollment) / classesWithCapacity.Sum(c => c.MaxStudents) * 100, 2)
                : 0;

            // Phân bố điểm số
            stats.ScoreStats = CalculateScoreDistribution(scores.ToList());

            // Phân bố theo Major
            stats.StudentsByMajor = students
                .GroupBy(s => GetMajorDisplayName(s.Major))
                .ToDictionary(g => g.Key, g => g.Count());

            // Thống kê Enrollment theo status
            stats.EnrollmentsByStatus = enrollments
                .GroupBy(e => e.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Top 10 học sinh xuất sắc
            stats.TopStudents = students
                .Where(s => s.OverallGPA > 0)
                .OrderByDescending(s => s.OverallGPA)
                .Take(10)
                .Select((s, index) => new TopStudent
                {
                    Rank = index + 1,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Major = GetMajorDisplayName(s.Major),
                    GPA = Math.Round(s.OverallGPA, 2)
                })
                .ToList();

            // Thống kê lớp học
            stats.ClassStatistics = classes
                .Select(c => new ClassStatistic
                {
                    ClassCode = c.ClassCode,
                    ClassName = c.ClassName,
                    CourseName = c.Course?.CourseName ?? "N/A",
                    MaxStudents = c.MaxStudents,
                    CurrentEnrollment = c.CurrentEnrollment,
                    FillRate = c.MaxStudents > 0 
                        ? Math.Round((double)c.CurrentEnrollment / c.MaxStudents * 100, 2) 
                        : 0,
                    Status = c.CurrentEnrollment >= c.MaxStudents ? "Full" : "Available"
                })
                .OrderByDescending(c => c.FillRate)
                .Take(15)
                .ToList();

            // Thống kê học kỳ hiện tại
            var currentSemester = classes.FirstOrDefault(c => c.Semester?.IsActive == true)?.Semester;
            if (currentSemester != null)
            {
                stats.CurrentSemesterClasses = classes.Count(c => c.SemesterId == currentSemester.SemesterId);
                stats.CurrentSemesterEnrollments = enrollments.Count(e => 
                    classes.Any(c => c.ClassId == e.ClassId && c.SemesterId == currentSemester.SemesterId));
            }

            return stats;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsByMajorAsync(string major)
        {
            var allStats = await GetDashboardStatisticsAsync();
            
            // Filter theo major
            var students = await _studentRepo.GetAllAsync();
            var filteredStudents = students.Where(s => GetMajorDisplayName(s.Major) == major).ToList();

            allStats.TotalStudents = filteredStudents.Count;
            allStats.AverageGPA = filteredStudents.Any() 
                ? Math.Round(filteredStudents.Average(s => s.OverallGPA), 2) 
                : 0;

            return allStats;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsBySemesterAsync(int semesterId)
        {
            var allStats = await GetDashboardStatisticsAsync();
            
            var classes = await _classRepo.GetAllAsync();
            var semesterClasses = classes.Where(c => c.SemesterId == semesterId).ToList();
            
            allStats.CurrentSemesterClasses = semesterClasses.Count;
            
            return allStats;
        }

        private ScoreDistribution CalculateScoreDistribution(List<Score> scores)
        {
            var distribution = new ScoreDistribution();

            foreach (var score in scores)
            {
                var value = score.ScoreValue;
                if (value >= 9.0) distribution.Excellent++;
                else if (value >= 8.0) distribution.VeryGood++;
                else if (value >= 7.0) distribution.Good++;
                else if (value >= 5.5) distribution.Average++;
                else if (value >= 4.0) distribution.BelowAverage++;
                else distribution.Poor++;
            }

            return distribution;
        }

        private string GetMajorDisplayName(DataEnums.MajorType major)
        {
            var displayAttr = major.GetType()
                .GetField(major.ToString())?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault();

            return displayAttr?.Name ?? major.ToString();
        }
    }
}
