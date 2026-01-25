using DataAccess.Entities;
using DataAccess.Enums;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Enrollment - Logic nghiệp vụ đăng ký lớp học
    /// </summary>
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository, 
            IClassRepository classRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _classRepository = classRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            try
            {
                return await _enrollmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            try
            {
                return await _enrollmentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        {
            try
            {
                return await _enrollmentRepository.GetByStudentAsync(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký theo sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetByClassAsync(int classId)
        {
            try
            {
                return await _enrollmentRepository.GetByClassAsync(classId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký theo lớp: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            try
            {
                // ✅ BƯỚC 1: Kiểm tra đã đăng ký chưa
                if (await _enrollmentRepository.IsEnrolledAsync(enrollment.StudentId, enrollment.ClassId))
                {
                    throw new Exception("Sinh viên đã đăng ký lớp này");
                }

                // Lấy thông tin lớp
                var classEntity = await _classRepository.GetByIdAsync(enrollment.ClassId);
                if (classEntity == null)
                {
                    throw new Exception("Không tìm thấy lớp học");
                }

                // ✅ BƯỚC 2: Kiểm tra ĐIỀU KIỆN TIÊN QUYẾT
                if (classEntity.Course?.PrerequisiteCourseId.HasValue == true)
                {
                    var hasPassedPrerequisite = await CheckPrerequisiteAsync(
                        enrollment.StudentId, 
                        classEntity.Course.PrerequisiteCourseId.Value);

                    if (!hasPassedPrerequisite)
                    {
                        var prerequisiteCourse = await _courseRepository.GetByIdAsync(classEntity.Course.PrerequisiteCourseId.Value);
                        var prerequisiteName = prerequisiteCourse?.CourseName ?? "môn tiên quyết";
                        throw new Exception($"Bạn chưa đủ điều kiện. Cần qua môn '{prerequisiteName}' trước.");
                    }
                }

                // ✅ BƯỚC 3: Kiểm tra SĨ SỐ LỚP
                if (classEntity.CurrentEnrollment >= classEntity.MaxCapacity)
                {
                    throw new Exception($"Lớp đã hết chỗ ({classEntity.CurrentEnrollment}/{classEntity.MaxCapacity}). Vui lòng chọn lớp khác.");
                }

                // ✅ BƯỚC 4: Kiểm tra TRÙNG LỊCH HỌC
                var hasScheduleConflict = await CheckScheduleConflictAsync(
                    enrollment.StudentId, 
                    classEntity.SemesterId, 
                    classEntity.DayOfWeekPair, 
                    classEntity.TimeSlot);

                if (hasScheduleConflict)
                {
                    throw new Exception($"Lịch học trùng với môn khác. Ca: {GetTimeSlotDescription(classEntity.TimeSlot)}, Ngày: {GetDayOfWeekDescription(classEntity.DayOfWeekPair)}");
                }

                // ✅ TẤT CẢ VALIDATION OK - Tạo Enrollment
                enrollment.EnrollmentDate = DateTime.Now;
                enrollment.Status = "Active";

                var result = await _enrollmentRepository.AddAsync(enrollment);

                // Cập nhật sĩ số lớp
                classEntity.CurrentEnrollment++;
                await _classRepository.UpdateAsync(classEntity);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng ký: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra sinh viên đã qua môn tiên quyết chưa
        /// </summary>
        private async Task<bool> CheckPrerequisiteAsync(int studentId, int prerequisiteCourseId)
        {
            var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);
            
            return enrollments.Any(e => 
                e.Class?.CourseId == prerequisiteCourseId && 
                e.IsPassed && 
                e.Grade != "F");
        }

        /// <summary>
        /// Kiểm tra trùng lịch học
        /// </summary>
        private async Task<bool> CheckScheduleConflictAsync(
            int studentId, 
            int semesterId, 
            DayOfWeekPair dayOfWeekPair, 
            TimeSlot timeSlot)
        {
            var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);

            // Lấy tất cả lớp đã đăng ký trong cùng học kỳ
            var enrolledClasses = enrollments
                .Where(e => e.Status == "Active" && e.Class?.SemesterId == semesterId)
                .Select(e => e.Class)
                .Where(c => c != null)
                .ToList();

            // Kiểm tra trùng lịch
            foreach (var enrolledClass in enrolledClasses)
            {
                // Nếu cùng ca học VÀ có ngày trùng nhau
                if (enrolledClass.TimeSlot == timeSlot && 
                    DoDayOfWeekPairsOverlap(enrolledClass.DayOfWeekPair, dayOfWeekPair))
                {
                    return true; // Có trùng lịch
                }
            }

            return false; // Không trùng
        }

        /// <summary>
        /// Kiểm tra 2 cặp ngày có trùng không (VD: 2-5 và 3-6 => trùng ngày 5-3)
        /// </summary>
        private bool DoDayOfWeekPairsOverlap(DayOfWeekPair pair1, DayOfWeekPair pair2)
        {
            // Parse pair1
            var (day1_1, day1_2) = ParseDayOfWeekPair(pair1);
            // Parse pair2
            var (day2_1, day2_2) = ParseDayOfWeekPair(pair2);

            // Kiểm tra trùng
            return day1_1 == day2_1 || day1_1 == day2_2 || 
                   day1_2 == day2_1 || day1_2 == day2_2;
        }

        /// <summary>
        /// Chuyển DayOfWeekPair enum thành 2 ngày cụ thể
        /// </summary>
        private (int, int) ParseDayOfWeekPair(DayOfWeekPair pair)
        {
            return pair switch
            {
                DayOfWeekPair.MonThu => (2, 5),    // Thứ 2 - Thứ 5
                DayOfWeekPair.TueFri => (3, 6),    // Thứ 3 - Thứ 6
                DayOfWeekPair.WedSat => (4, 7),    // Thứ 4 - Thứ 7
                _ => (0, 0)
            };
        }

        /// <summary>
        /// Mô tả ca học
        /// </summary>
        private string GetTimeSlotDescription(TimeSlot slot)
        {
            return slot switch
            {
                TimeSlot.Slot1 => "Ca 1 (7:30-9:50)",
                TimeSlot.Slot2 => "Ca 2 (10:00-12:20)",
                TimeSlot.Slot3 => "Ca 3 (12:50-15:10)",
                TimeSlot.Slot4 => "Ca 4 (15:20-17:40)",
                _ => "Không rõ"
            };
        }

        /// <summary>
        /// Mô tả cặp ngày
        /// </summary>
        private string GetDayOfWeekDescription(DayOfWeekPair pair)
        {
            return pair switch
            {
                DayOfWeekPair.MonThu => "Thứ 2 - Thứ 5",
                DayOfWeekPair.TueFri => "Thứ 3 - Thứ 6",
                DayOfWeekPair.WedSat => "Thứ 4 - Thứ 7",
                _ => "Không rõ"
            };
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            try
            {
                return await _enrollmentRepository.UpdateAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật đăng ký: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật điểm cho sinh viên
        /// </summary>
        public async Task<Enrollment> UpdateGradeAsync(int enrollmentId, double? midterm, double? final)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                {
                    throw new Exception("Không tìm thấy đăng ký");
                }

                enrollment.MidtermScore = midterm;
                enrollment.FinalScore = final;

                // Tính điểm tổng kết (40% Midterm + 60% Final)
                if (midterm.HasValue && final.HasValue)
                {
                    enrollment.TotalScore = midterm.Value * 0.4 + final.Value * 0.6;
                    
                    // Tính Grade
                    enrollment.Grade = CalculateGrade(enrollment.TotalScore.Value);
                    enrollment.IsPassed = enrollment.TotalScore.Value >= 5.0;
                }

                return await _enrollmentRepository.UpdateAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật điểm: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment != null)
                {
                    // Giảm sĩ số lớp
                    var classEntity = await _classRepository.GetByIdAsync(enrollment.ClassId);
                    if (classEntity != null && classEntity.CurrentEnrollment > 0)
                    {
                        classEntity.CurrentEnrollment--;
                        await _classRepository.UpdateAsync(classEntity);
                    }
                }

                await _enrollmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsEnrolledAsync(int studentId, int classId)
        {
            try
            {
                return await _enrollmentRepository.IsEnrolledAsync(studentId, classId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment> ChangeClassAsync(int enrollmentId, int newClassId)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
                if (enrollment == null) throw new Exception("Không tìm thấy đăng ký");

                var oldClass = await _classRepository.GetByIdAsync(enrollment.ClassId);
                var newClass = await _classRepository.GetByIdAsync(newClassId);

                if (newClass == null) throw new Exception("Không tìm thấy lớp mới");
                if (oldClass != null && oldClass.CourseId != newClass.CourseId) throw new Exception("Lớp mới phải thuộc cùng môn học");
                if (newClass.CurrentEnrollment >= newClass.MaxCapacity) throw new Exception("Lớp mới đã đầy");

                // Update counts
                if (oldClass != null) 
                {
                    oldClass.CurrentEnrollment--;
                    await _classRepository.UpdateAsync(oldClass);
                }
                
                newClass.CurrentEnrollment++;
                await _classRepository.UpdateAsync(newClass);

                enrollment.ClassId = newClassId;
                return await _enrollmentRepository.UpdateAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi chuyển lớp: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính Grade từ điểm tổng kết
        /// </summary>
        private string CalculateGrade(double totalScore)
        {
            if (totalScore >= 9.0) return "A+";
            if (totalScore >= 8.5) return "A";
            if (totalScore >= 8.0) return "B+";
            if (totalScore >= 7.0) return "B";
            if (totalScore >= 6.5) return "C+";
            if (totalScore >= 5.5) return "C";
            if (totalScore >= 5.0) return "D+";
            if (totalScore >= 4.0) return "D";
            return "F";
        }
    }
}
