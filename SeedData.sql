-- ============================================
-- SQL SCRIPT: SEED DATA - 30 RECORDS MỖI BẢNG
-- Database: StudentManagementDB
-- Chạy script này trong MySQL Workbench hoặc terminal
-- ============================================

USE StudentManagementDB;

-- ============================================
-- XÓA DỮ LIỆU CŨ (theo thứ tự foreign key)
-- ============================================
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE LearningPathRecommendations;
TRUNCATE TABLE AcademicAnalyses;
TRUNCATE TABLE Notifications;
TRUNCATE TABLE Scores;
TRUNCATE TABLE Enrollments;
TRUNCATE TABLE Classes;
TRUNCATE TABLE Courses;
TRUNCATE TABLE Semesters;
TRUNCATE TABLE Students;
TRUNCATE TABLE Users;
TRUNCATE TABLE Roles;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================
-- 1. ROLES (3 roles cơ bản)
-- ============================================
INSERT INTO Roles (RoleName, Description) VALUES
('Admin', 'Quản trị viên hệ thống'),
('Teacher', 'Giảng viên'),
('Student', 'Sinh viên');

-- ============================================
-- 2. USERS (30 users: 1 Admin, 9 Teachers, 20 Students)
-- ============================================
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, RoleId, IsActive, CreatedAt, MustChangePassword) VALUES
-- Admin
('Administrator', 'admin@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0999999999', 1, 1, NOW(), 0),

-- Teachers (9)
('Nguyễn Văn A', 'teachera@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901111111', 2, 1, NOW(), 0),
('Trần Thị B', 'teacherb@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0902222222', 2, 1, NOW(), 0),
('Lê Văn C', 'teacherc@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0903333333', 2, 1, NOW(), 0),
('Phạm Thị D', 'teacherd@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0904444444', 2, 1, NOW(), 0),
('Hoàng Văn E', 'teachere@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0905555555', 2, 1, NOW(), 0),
('Vũ Thị F', 'teacherf@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0906666666', 2, 1, NOW(), 0),
('Đặng Văn G', 'teacherg@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0907777777', 2, 1, NOW(), 0),
('Bùi Thị H', 'teacherh@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0908888888', 2, 1, NOW(), 0),
('Dương Văn I', 'teacheri@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0909999999', 2, 1, NOW(), 0),

-- Students (20)
('Ngô Minh J', 'student001@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234567', 3, 1, NOW(), 1),
('Lý Thị K', 'student002@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234568', 3, 1, NOW(), 1),
('Đinh Văn L', 'student003@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234569', 3, 1, NOW(), 1),
('Võ Thị M', 'student004@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234570', 3, 1, NOW(), 1),
('Trương Văn N', 'student005@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234571', 3, 1, NOW(), 1),
('Phan Thị O', 'student006@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234572', 3, 1, NOW(), 1),
('Mai Văn P', 'student007@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234573', 3, 1, NOW(), 1),
('Tô Thị Q', 'student008@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234574', 3, 1, NOW(), 1),
('Hồ Văn R', 'student009@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234575', 3, 1, NOW(), 1),
('Lâm Thị S', 'student010@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234576', 3, 1, NOW(), 1),
('Cao Văn T', 'student011@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234577', 3, 1, NOW(), 1),
('Đỗ Thị U', 'student012@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234578', 3, 1, NOW(), 1),
('Từ Văn V', 'student013@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234579', 3, 1, NOW(), 1),
('Lưu Thị W', 'student014@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234580', 3, 1, NOW(), 1),
('Nguyễn Văn X', 'student015@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234581', 3, 1, NOW(), 1),
('Trần Thị Y', 'student016@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234582', 3, 1, NOW(), 1),
('Lê Văn Z', 'student017@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234583', 3, 1, NOW(), 1),
('Phạm Thị AA', 'student018@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234584', 3, 1, NOW(), 1),
('Hoàng Văn BB', 'student019@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234585', 3, 1, NOW(), 1),
('Vũ Thị CC', 'student020@fpt.edu.vn', '$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ', '0901234586', 3, 1, NOW(), 1);

-- ============================================
-- 3. STUDENTS (30 students)
-- ============================================
INSERT INTO Students (StudentCode, FullName, Email, PhoneNumber, DateOfBirth, ClassCode, OverallGPA, CreatedAt, Major, CurrentTermNo, IsFirstLogin, UserId) VALUES
('STU202600001', 'Ngô Minh J', 'student001@fpt.edu.vn', '0901234567', '2005-01-15', 'SE1801', 0, NOW(), 'SE', 1, 1, 11),
('STU202600002', 'Lý Thị K', 'student002@fpt.edu.vn', '0901234568', '2005-02-20', 'SE1801', 0, NOW(), 'SE', 1, 1, 12),
('STU202600003', 'Đinh Văn L', 'student003@fpt.edu.vn', '0901234569', '2005-03-25', 'SE1802', 0, NOW(), 'SE', 1, 1, 13),
('STU202600004', 'Võ Thị M', 'student004@fpt.edu.vn', '0901234570', '2005-04-10', 'SE1802', 0, NOW(), 'SE', 1, 1, 14),
('STU202600005', 'Trương Văn N', 'student005@fpt.edu.vn', '0901234571', '2005-05-12', 'IA1801', 0, NOW(), 'IA', 1, 1, 15),
('STU202600006', 'Phan Thị O', 'student006@fpt.edu.vn', '0901234572', '2005-06-18', 'IA1801', 0, NOW(), 'IA', 1, 1, 16),
('STU202600007', 'Mai Văn P', 'student007@fpt.edu.vn', '0901234573', '2005-07-22', 'IA1802', 0, NOW(), 'IA', 1, 1, 17),
('STU202600008', 'Tô Thị Q', 'student008@fpt.edu.vn', '0901234574', '2005-08-05', 'IA1802', 0, NOW(), 'IA', 1, 1, 18),
('STU202600009', 'Hồ Văn R', 'student009@fpt.edu.vn', '0901234575', '2005-09-14', 'IS1801', 0, NOW(), 'IS', 1, 1, 19),
('STU202600010', 'Lâm Thị S', 'student010@fpt.edu.vn', '0901234576', '2005-10-19', 'IS1801', 0, NOW(), 'IS', 1, 1, 20),
('STU202600011', 'Cao Văn T', 'student011@fpt.edu.vn', '0901234577', '2005-11-23', 'IS1802', 0, NOW(), 'IS', 1, 1, 21),
('STU202600012', 'Đỗ Thị U', 'student012@fpt.edu.vn', '0901234578', '2005-12-28', 'IS1802', 0, NOW(), 'IS', 1, 1, 22),
('STU202600013', 'Từ Văn V', 'student013@fpt.edu.vn', '0901234579', '2005-01-30', 'SE1803', 0, NOW(), 'SE', 1, 1, 23),
('STU202600014', 'Lưu Thị W', 'student014@fpt.edu.vn', '0901234580', '2005-02-14', 'SE1803', 0, NOW(), 'SE', 1, 1, 24),
('STU202600015', 'Nguyễn Văn X', 'student015@fpt.edu.vn', '0901234581', '2005-03-17', 'SE1804', 0, NOW(), 'SE', 1, 1, 25),
('STU202600016', 'Trần Thị Y', 'student016@fpt.edu.vn', '0901234582', '2005-04-21', 'SE1804', 0, NOW(), 'SE', 1, 1, 26),
('STU202600017', 'Lê Văn Z', 'student017@fpt.edu.vn', '0901234583', '2005-05-26', 'IA1803', 0, NOW(), 'IA', 1, 1, 27),
('STU202600018', 'Phạm Thị AA', 'student018@fpt.edu.vn', '0901234584', '2005-06-30', 'IA1803', 0, NOW(), 'IA', 1, 1, 28),
('STU202600019', 'Hoàng Văn BB', 'student019@fpt.edu.vn', '0901234585', '2005-07-04', 'IA1804', 0, NOW(), 'IA', 1, 1, 29),
('STU202600020', 'Vũ Thị CC', 'student020@fpt.edu.vn', '0901234586', '2005-08-08', 'IA1804', 0, NOW(), 'IA', 1, 1, 30),
('STU202600021', 'Nguyễn Văn DD', 'student021@fpt.edu.vn', '0901234587', '2005-09-11', 'IS1803', 0, NOW(), 'IS', 1, 1, 11),
('STU202600022', 'Trần Thị EE', 'student022@fpt.edu.vn', '0901234588', '2005-10-15', 'IS1803', 0, NOW(), 'IS', 1, 1, 12),
('STU202600023', 'Lê Văn FF', 'student023@fpt.edu.vn', '0901234589', '2005-11-19', 'IS1804', 0, NOW(), 'IS', 1, 1, 13),
('STU202600024', 'Phạm Thị GG', 'student024@fpt.edu.vn', '0901234590', '2005-12-23', 'IS1804', 0, NOW(), 'IS', 1, 1, 14),
('STU202600025', 'Hoàng Văn HH', 'student025@fpt.edu.vn', '0901234591', '2005-01-27', 'SE1805', 0, NOW(), 'SE', 1, 1, 15),
('STU202600026', 'Vũ Thị II', 'student026@fpt.edu.vn', '0901234592', '2005-02-01', 'SE1805', 0, NOW(), 'SE', 1, 1, 16),
('STU202600027', 'Đặng Văn JJ', 'student027@fpt.edu.vn', '0901234593', '2005-03-06', 'SE1806', 0, NOW(), 'SE', 1, 1, 17),
('STU202600028', 'Bùi Thị KK', 'student028@fpt.edu.vn', '0901234594', '2005-04-10', 'SE1806', 0, NOW(), 'SE', 1, 1, 18),
('STU202600029', 'Dương Văn LL', 'student029@fpt.edu.vn', '0901234595', '2005-05-14', 'IA1805', 0, NOW(), 'IA', 1, 1, 19),
('STU202600030', 'Ngô Thị MM', 'student030@fpt.edu.vn', '0901234596', '2005-06-18', 'IA1805', 0, NOW(), 'IA', 1, 1, 20);

-- ============================================
-- 4. SEMESTERS (30 semesters - 10 năm)
-- ============================================
INSERT INTO Semesters (SemesterCode, SemesterName, StartDate, EndDate, IsActive) VALUES
('FA2020', 'Fall 2020', '2020-09-01', '2021-01-15', 0),
('SP2021', 'Spring 2021', '2021-02-01', '2021-06-15', 0),
('SU2021', 'Summer 2021', '2021-07-01', '2021-08-30', 0),
('FA2021', 'Fall 2021', '2021-09-01', '2022-01-15', 0),
('SP2022', 'Spring 2022', '2022-02-01', '2022-06-15', 0),
('SU2022', 'Summer 2022', '2022-07-01', '2022-08-30', 0),
('FA2022', 'Fall 2022', '2022-09-01', '2023-01-15', 0),
('SP2023', 'Spring 2023', '2023-02-01', '2023-06-15', 0),
('SU2023', 'Summer 2023', '2023-07-01', '2023-08-30', 0),
('FA2023', 'Fall 2023', '2023-09-01', '2024-01-15', 0),
('SP2024', 'Spring 2024', '2024-02-01', '2024-06-15', 0),
('SU2024', 'Summer 2024', '2024-07-01', '2024-08-30', 0),
('FA2024', 'Fall 2024', '2024-09-01', '2025-01-15', 0),
('SP2025', 'Spring 2025', '2025-02-01', '2025-06-15', 0),
('SU2025', 'Summer 2025', '2025-07-01', '2025-08-30', 0),
('FA2025', 'Fall 2025', '2025-09-01', '2026-01-15', 1), -- Active semester
('SP2026', 'Spring 2026', '2026-02-01', '2026-06-15', 0),
('SU2026', 'Summer 2026', '2026-07-01', '2026-08-30', 0),
('FA2026', 'Fall 2026', '2026-09-01', '2027-01-15', 0),
('SP2027', 'Spring 2027', '2027-02-01', '2027-06-15', 0),
('SU2027', 'Summer 2027', '2027-07-01', '2027-08-30', 0),
('FA2027', 'Fall 2027', '2027-09-01', '2028-01-15', 0),
('SP2028', 'Spring 2028', '2028-02-01', '2028-06-15', 0),
('SU2028', 'Summer 2028', '2028-07-01', '2028-08-30', 0),
('FA2028', 'Fall 2028', '2028-09-01', '2029-01-15', 0),
('SP2029', 'Spring 2029', '2029-02-01', '2029-06-15', 0),
('SU2029', 'Summer 2029', '2029-07-01', '2029-08-30', 0),
('FA2029', 'Fall 2029', '2029-09-01', '2030-01-15', 0),
('SP2030', 'Spring 2030', '2030-02-01', '2030-06-15', 0),
('SU2030', 'Summer 2030', '2030-07-01', '2030-08-30', 0);

-- ============================================
-- 5. COURSES (30 courses)
-- ============================================
INSERT INTO Courses (CourseName, CourseCode, Credits, Major, PrerequisiteCourseId) VALUES
('Lập trình C', 'PRN101', 3, 'SE', NULL),
('Cấu trúc dữ liệu', 'DSA101', 3, 'SE', 1),
('Lập trình Java', 'PRN211', 3, 'SE', 1),
('Lập trình C#', 'PRN212', 3, 'SE', 1),
('Cơ sở dữ liệu', 'DBI202', 3, 'SE', NULL),
('Đồ án 1', 'PRJ301', 3, 'SE', 3),
('Toán rời rạc', 'MAD101', 3, 'SE', NULL),
('Xác suất thống kê', 'STA101', 3, 'SE', NULL),
('Vật lý đại cương', 'PHY101', 2, 'SE', NULL),
('Anh văn 1', 'ENG101', 2, NULL, NULL),
('Anh văn 2', 'ENG102', 2, NULL, 10),
('Anh văn 3', 'ENG103', 2, NULL, 11),
('Giáo dục thể chất', 'PED101', 1, NULL, NULL),
('Trí tuệ nhân tạo cơ bản', 'AIF101', 3, 'IA', NULL),
('Machine Learning', 'AIL302', 3, 'IA', 14),
('Deep Learning', 'AIL303', 3, 'IA', 15),
('Computer Vision', 'AIC304', 3, 'IA', 16),
('Natural Language Processing', 'AIN305', 3, 'IA', 16),
('Hệ điều hành', 'OSG202', 3, 'SE', 1),
('Mạng máy tính', 'NWC203', 3, 'SE', NULL),
('An toàn thông tin', 'SEC301', 3, 'IS', 20),
('Quản trị hệ thống', 'SYS302', 3, 'IS', 19),
('Phân tích thiết kế hệ thống', 'SAD201', 3, 'IS', 5),
('Quản lý dự án phần mềm', 'SWP391', 3, 'SE', NULL),
('Kiểm thử phần mềm', 'SWT301', 3, 'SE', 6),
('Kiến trúc phần mềm', 'SAR302', 3, 'SE', 6),
('DevOps', 'DOP401', 3, 'SE', 19),
('Cloud Computing', 'CLO402', 3, 'SE', 20),
('Blockchain', 'BLC403', 3, 'SE', 5),
('IoT & Embedded Systems', 'IOT404', 3, 'SE', 19);

-- ============================================
-- 6. CLASSES (30 classes)
-- ============================================
INSERT INTO Classes (ClassName, ClassCode, MaxCapacity, CurrentEnrollment, Room, Schedule, DayOfWeekPair, TimeSlot, CourseId, SemesterId, TeacherId) VALUES
-- Fall 2025 (SemesterId = 16)
('Lập trình C - Lớp 1', 'PRN101-01', 40, 0, 'P.304', 'Mon-Thu (Slot 1)', 1, 1, 1, 16, 2),
('Lập trình C - Lớp 2', 'PRN101-02', 40, 0, 'P.305', 'Tue-Fri (Slot 2)', 2, 2, 1, 16, 3),
('Cấu trúc dữ liệu - Lớp 1', 'DSA101-01', 35, 0, 'P.306', 'Wed-Sat (Slot 3)', 3, 3, 2, 16, 4),
('Lập trình Java - Lớp 1', 'PRN211-01', 40, 0, 'P.307', 'Mon-Thu (Slot 1)', 1, 1, 3, 16, 5),
('Lập trình Java - Lớp 2', 'PRN211-02', 40, 0, 'P.308', 'Tue-Fri (Slot 2)', 2, 2, 3, 16, 2),
('Lập trình C# - Lớp 1', 'PRN212-01', 35, 0, 'P.309', 'Wed-Sat (Slot 4)', 3, 4, 4, 16, 3),
('Cơ sở dữ liệu - Lớp 1', 'DBI202-01', 40, 0, 'P.310', 'Mon-Thu (Slot 2)', 1, 2, 5, 16, 4),
('Cơ sở dữ liệu - Lớp 2', 'DBI202-02', 40, 0, 'P.311', 'Tue-Fri (Slot 3)', 2, 3, 5, 16, 5),
('Đồ án 1 - Lớp 1', 'PRJ301-01', 30, 0, 'P.312', 'Wed-Sat (Slot 1)', 3, 1, 6, 16, 6),
('Toán rời rạc - Lớp 1', 'MAD101-01', 40, 0, 'P.313', 'Mon-Thu (Slot 3)', 1, 3, 7, 16, 7),
('Xác suất thống kê - Lớp 1', 'STA101-01', 40, 0, 'P.314', 'Tue-Fri (Slot 4)', 2, 4, 8, 16, 8),
('Vật lý đại cương - Lớp 1', 'PHY101-01', 45, 0, 'P.315', 'Wed-Sat (Slot 2)', 3, 2, 9, 16, 9),
('Anh văn 1 - Lớp 1', 'ENG101-01', 30, 0, 'P.316', 'Mon-Thu (Slot 4)', 1, 4, 10, 16, 10),
('Anh văn 1 - Lớp 2', 'ENG101-02', 30, 0, 'P.317', 'Tue-Fri (Slot 1)', 2, 1, 10, 16, 2),
('Anh văn 2 - Lớp 1', 'ENG102-01', 30, 0, 'P.318', 'Wed-Sat (Slot 3)', 3, 3, 11, 16, 3),
('Giáo dục thể chất - Lớp 1', 'PED101-01', 50, 0, 'Sân', 'Mon-Thu (Slot 2)', 1, 2, 13, 16, 4),
('AI cơ bản - Lớp 1', 'AIF101-01', 35, 0, 'P.401', 'Tue-Fri (Slot 2)', 2, 2, 14, 16, 5),
('Machine Learning - Lớp 1', 'AIL302-01', 30, 0, 'P.402', 'Wed-Sat (Slot 4)', 3, 4, 15, 16, 6),
('Deep Learning - Lớp 1', 'AIL303-01', 25, 0, 'P.403', 'Mon-Thu (Slot 1)', 1, 1, 16, 16, 7),
('Computer Vision - Lớp 1', 'AIC304-01', 25, 0, 'P.404', 'Tue-Fri (Slot 3)', 2, 3, 17, 16, 8),
('NLP - Lớp 1', 'AIN305-01', 25, 0, 'P.405', 'Wed-Sat (Slot 1)', 3, 1, 18, 16, 9),
('Hệ điều hành - Lớp 1', 'OSG202-01', 40, 0, 'P.406', 'Mon-Thu (Slot 3)', 1, 3, 19, 16, 10),
('Mạng máy tính - Lớp 1', 'NWC203-01', 40, 0, 'P.407', 'Tue-Fri (Slot 4)', 2, 4, 20, 16, 2),
('An toàn thông tin - Lớp 1', 'SEC301-01', 35, 0, 'P.408', 'Wed-Sat (Slot 2)', 3, 2, 21, 16, 3),
('Quản trị hệ thống - Lớp 1', 'SYS302-01', 30, 0, 'P.409', 'Mon-Thu (Slot 4)', 1, 4, 22, 16, 4),
('Phân tích thiết kế - Lớp 1', 'SAD201-01', 35, 0, 'P.410', 'Tue-Fri (Slot 1)', 2, 1, 23, 16, 5),
('Quản lý dự án - Lớp 1', 'SWP391-01', 30, 0, 'P.411', 'Wed-Sat (Slot 3)', 3, 3, 24, 16, 6),
('Kiểm thử phần mềm - Lớp 1', 'SWT301-01', 35, 0, 'P.412', 'Mon-Thu (Slot 2)', 1, 2, 25, 16, 7),
('DevOps - Lớp 1', 'DOP401-01', 30, 0, 'P.413', 'Tue-Fri (Slot 3)', 2, 3, 27, 16, 8),
('Cloud Computing - Lớp 1', 'CLO402-01', 30, 0, 'P.414', 'Wed-Sat (Slot 4)', 3, 4, 28, 16, 9);

-- ============================================
-- 7. ENROLLMENTS (30 enrollments - sample data)
-- ============================================
INSERT INTO Enrollments (StudentId, ClassId, EnrollmentDate, Status, MidtermScore, FinalScore, TotalScore, Grade, IsPassed, AttemptNumber) VALUES
(1, 1, '2025-09-05', 'Active', 8.5, 9.0, 8.8, 'A', 1, 1),
(2, 1, '2025-09-05', 'Active', 7.0, 7.5, 7.3, 'B', 1, 1),
(3, 2, '2025-09-05', 'Active', 6.5, 7.0, 6.8, 'C+', 1, 1),
(4, 2, '2025-09-05', 'Active', 9.0, 9.5, 9.3, 'A+', 1, 1),
(5, 3, '2025-09-05', 'Active', 5.5, 6.0, 5.8, 'C', 1, 1),
(6, 3, '2025-09-05', 'Active', 8.0, 8.5, 8.3, 'A', 1, 1),
(7, 4, '2025-09-05', 'Active', 7.5, 8.0, 7.8, 'B+', 1, 1),
(8, 4, '2025-09-05', 'Active', 4.0, 4.5, 4.3, 'D', 1, 1),
(9, 5, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(10, 5, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(11, 6, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(12, 6, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(13, 7, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(14, 7, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(15, 8, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(16, 8, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(17, 9, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(18, 9, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(19, 10, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(20, 10, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(21, 11, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(22, 11, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(23, 12, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(24, 12, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(25, 13, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(26, 13, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(27, 14, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(28, 14, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(29, 15, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1),
(30, 15, '2025-09-05', 'Active', NULL, NULL, NULL, NULL, 0, 1);

-- ============================================
-- 8. SCORES (30 scores - sample data)
-- ============================================
INSERT INTO Scores (StudentId, CourseId, ScoreValue) VALUES
(1, 1, 8.8),
(2, 1, 7.3),
(3, 1, 6.8),
(4, 1, 9.3),
(5, 2, 5.8),
(6, 2, 8.3),
(7, 3, 7.8),
(8, 3, 4.3),
(9, 4, 8.5),
(10, 4, 7.0),
(11, 5, 9.0),
(12, 5, 6.5),
(13, 7, 7.5),
(14, 7, 8.0),
(15, 8, 6.0),
(16, 8, 7.5),
(17, 9, 8.5),
(18, 9, 5.5),
(19, 10, 9.0),
(20, 10, 8.0),
(21, 11, 7.5),
(22, 11, 6.5),
(23, 13, 9.5),
(24, 13, 8.5),
(25, 14, 7.0),
(26, 14, 8.5),
(27, 15, 9.0),
(28, 15, 6.0),
(29, 16, 8.0),
(30, 16, 7.5);

-- ============================================
-- 9. NOTIFICATIONS (30 notifications)
-- ============================================
INSERT INTO Notifications (StudentId, Title, Message, Type, IsRead, CreatedAt) VALUES
(1, 'Chào mừng!', 'Chào mừng bạn đến với hệ thống quản lý sinh viên', 'Welcome', 1, NOW()),
(2, 'Đăng ký thành công', 'Bạn đã đăng ký môn Lập trình C thành công', 'EnrollmentSuccess', 1, NOW()),
(3, 'Điểm mới', 'Điểm môn Lập trình C đã được cập nhật', 'ScoreUpdate', 0, NOW()),
(4, 'AI Phân tích', 'Hệ thống AI đã phân tích kết quả học tập của bạn', 'AIAnalysis', 0, NOW()),
(5, 'Chào mừng!', 'Chào mừng bạn đến với hệ thống', 'Welcome', 1, NOW()),
(6, 'Đăng ký thành công', 'Đã đăng ký Cấu trúc dữ liệu', 'EnrollmentSuccess', 1, NOW()),
(7, 'Điểm mới', 'Điểm Java đã có', 'ScoreUpdate', 0, NOW()),
(8, 'Cảnh báo', 'GPA thấp, cần cải thiện', 'Warning', 0, NOW()),
(9, 'Lộ trình học tập', 'AI đã đề xuất lộ trình cho bạn', 'LearningPath', 0, NOW()),
(10, 'Nhắc nhở', 'Deadline nộp bài tập', 'Reminder', 0, NOW()),
(11, 'Chào mừng!', 'Chào mừng sinh viên mới', 'Welcome', 1, NOW()),
(12, 'Đăng ký thành công', 'Đã đăng ký Database', 'EnrollmentSuccess', 1, NOW()),
(13, 'Điểm mới', 'Điểm Toán đã cập nhật', 'ScoreUpdate', 0, NOW()),
(14, 'AI Phân tích', 'Phân tích AI sẵn sàng', 'AIAnalysis', 0, NOW()),
(15, 'Thông báo', 'Lịch thi đã ra', 'General', 0, NOW()),
(16, 'Đăng ký thành công', 'Đã đăng ký Vật lý', 'EnrollmentSuccess', 1, NOW()),
(17, 'Điểm mới', 'Điểm English đã có', 'ScoreUpdate', 0, NOW()),
(18, 'Lộ trình học tập', 'Gợi ý môn học kỳ tới', 'LearningPath', 0, NOW()),
(19, 'Chào mừng!', 'Welcome to system', 'Welcome', 1, NOW()),
(20, 'Nhắc nhở', 'Đăng ký môn học', 'Reminder', 0, NOW()),
(21, 'Điểm mới', 'Điểm AI đã cập nhật', 'ScoreUpdate', 0, NOW()),
(22, 'Cảnh báo', 'Cần cải thiện môn yếu', 'Warning', 0, NOW()),
(23, 'Đăng ký thành công', 'Đã đăng ký ML', 'EnrollmentSuccess', 1, NOW()),
(24, 'AI Phân tích', 'Kết quả phân tích mới', 'AIAnalysis', 0, NOW()),
(25, 'Thông báo', 'Thông báo chung', 'General', 0, NOW()),
(26, 'Lộ trình học tập', 'Đề xuất môn học', 'LearningPath', 0, NOW()),
(27, 'Điểm mới', 'Điểm DL đã ra', 'ScoreUpdate', 0, NOW()),
(28, 'Nhắc nhở', 'Hạn nộp project', 'Reminder', 0, NOW()),
(29, 'Đăng ký thành công', 'Đã đăng ký CV', 'EnrollmentSuccess', 1, NOW()),
(30, 'Chào mừng!', 'Chúc học tập tốt', 'Welcome', 1, NOW());

-- ============================================
-- 10. ACADEMIC ANALYSES (30 analyses)
-- ============================================
INSERT INTO AcademicAnalyses (StudentId, AnalysisDate, OverallGPA, StrongSubjectsJson, WeakSubjectsJson, Recommendations, AiModelUsed) VALUES
(1, NOW(), 8.8, '["Lập trình C", "Java"]', '[]', 'Duy trì phong độ học tập tốt', 'Gemini-Pro'),
(2, NOW(), 7.3, '["Database"]', '["Toán"]', 'Cần ôn tập Toán', 'Gemini-Pro'),
(3, NOW(), 6.8, '[]', '["Lập trình"]', 'Tham gia lớp bổ trợ Lập trình', 'Gemini-Pro'),
(4, NOW(), 9.3, '["Lập trình C", "Database", "English"]', '[]', 'Xuất sắc! Tiếp tục phát huy', 'Gemini-Pro'),
(5, NOW(), 5.8, '[]', '["Cấu trúc dữ liệu", "Toán"]', 'Cần cố gắng hơn', 'Gemini-Pro'),
(6, NOW(), 8.3, '["Database", "Java"]', '[]', 'Học tập tốt', 'Gemini-Pro'),
(7, NOW(), 7.8, '["Java"]', '["Vật lý"]', 'Ôn Vật lý thêm', 'Gemini-Pro'),
(8, NOW(), 4.3, '[]', '["Java", "Database"]', 'Cảnh báo GPA thấp', 'Fallback'),
(9, NOW(), 8.5, '["C#", "AI"]', '[]', 'Rất tốt', 'Gemini-Pro'),
(10, NOW(), 7.0, '["Database"]', '["English"]', 'Cải thiện English', 'Gemini-Pro'),
(11, NOW(), 9.0, '["Database", "Network"]', '[]', 'Xuất sắc', 'Gemini-Pro'),
(12, NOW(), 6.5, '[]', '["Math"]', 'Học thêm Math', 'Gemini-Pro'),
(13, NOW(), 7.5, '["Math"]', '[]', 'Tốt', 'Gemini-Pro'),
(14, NOW(), 8.0, '["Statistics"]', '[]', 'Rất tốt', 'Gemini-Pro'),
(15, NOW(), 6.0, '[]', '["Physics"]', 'Ôn Physics', 'Gemini-Pro'),
(16, NOW(), 7.5, '["English"]', '[]', 'Tiếp tục', 'Gemini-Pro'),
(17, NOW(), 8.5, '["AI", "ML"]', '[]', 'Xuất sắc về AI', 'Gemini-Pro'),
(18, NOW(), 5.5, '[]', '["AI"]', 'Học thêm AI', 'Gemini-Pro'),
(19, NOW(), 9.0, '["English", "PE"]', '[]', 'Tốt lắm', 'Gemini-Pro'),
(20, NOW(), 8.0, '["English"]', '[]', 'Duy trì', 'Gemini-Pro'),
(21, NOW(), 7.5, '["ML"]', '[]', 'Khá tốt', 'Gemini-Pro'),
(22, NOW(), 6.5, '[]', '["Statistics"]', 'Cải thiện Stats', 'Gemini-Pro'),
(23, NOW(), 9.5, '["PE", "English"]', '[]', 'Hoàn hảo', 'Gemini-Pro'),
(24, NOW(), 8.5, '["AI"]', '[]', 'Rất tốt', 'Gemini-Pro'),
(25, NOW(), 7.0, '[]', '["Database"]', 'Ôn Database', 'Gemini-Pro'),
(26, NOW(), 8.5, '["ML", "AI"]', '[]', 'Xuất sắc AI/ML', 'Gemini-Pro'),
(27, NOW(), 9.0, '["DL", "CV"]', '[]', 'Rất giỏi DL', 'Gemini-Pro'),
(28, NOW(), 6.0, '[]', '["CV"]', 'Học thêm CV', 'Gemini-Pro'),
(29, NOW(), 8.0, '["NLP"]', '[]', 'Tốt về NLP', 'Gemini-Pro'),
(30, NOW(), 7.5, '["OS"]', '[]', 'Khá', 'Gemini-Pro');

-- ============================================
-- 11. LEARNING PATH RECOMMENDATIONS (30 recommendations)
-- ============================================
INSERT INTO LearningPathRecommendations (StudentId, SemesterId, RecommendationDate, RecommendedCoursesJson, OverallStrategy, WarningsJson, AiModelUsed, IsViewed) VALUES
(1, 16, NOW(), '[{"CourseName":"Machine Learning","Priority":1,"Reason":"Phù hợp với điểm mạnh"}]', 'Tiếp tục học AI/ML', '[]', 'Gemini', 0),
(2, 16, NOW(), '[{"CourseName":"Advanced Java","Priority":1,"Reason":"Nâng cao Java"}]', 'Củng cố Java', '[]', 'Gemini', 0),
(3, 16, NOW(), '[{"CourseName":"Database Advanced","Priority":1,"Reason":"Cải thiện DB"}]', 'Học DB', '[]', 'Gemini', 0),
(4, 16, NOW(), '[{"CourseName":"Deep Learning","Priority":1,"Reason":"Tiếp tục AI"}]', 'Phát triển AI', '[]', 'Gemini', 0),
(5, 16, NOW(), '[{"CourseName":"Toán nâng cao","Priority":1,"Reason":"Cải thiện nền tảng"}]', 'Củng cố Toán', '["Cần cải thiện GPA"]', 'Gemini', 0),
(6, 16, NOW(), '[{"CourseName":"Web Development","Priority":1,"Reason":"Mở rộng"}]', 'Học Web', '[]', 'Gemini', 0),
(7, 16, NOW(), '[{"CourseName":"Mobile Dev","Priority":1,"Reason":"Học Mobile"}]', 'Phát triển Mobile', '[]', 'Gemini', 0),
(8, 16, NOW(), '[{"CourseName":"Basic Programming","Priority":1,"Reason":"Củng cố cơ bản"}]', 'Học lại cơ bản', '["GPA thấp"]', 'Fallback', 0),
(9, 16, NOW(), '[{"CourseName":"Cloud Computing","Priority":1,"Reason":"Xu hướng mới"}]', 'Học Cloud', '[]', 'Gemini', 0),
(10, 16, NOW(), '[{"CourseName":"DevOps","Priority":1,"Reason":"Thực tiễn"}]', 'Học DevOps', '[]', 'Gemini', 0),
(11, 16, NOW(), '[{"CourseName":"Security","Priority":1,"Reason":"An ninh"}]', 'Học Security', '[]', 'Gemini', 0),
(12, 16, NOW(), '[{"CourseName":"Network Admin","Priority":1,"Reason":"Quản trị"}]', 'Học Admin', '[]', 'Gemini', 0),
(13, 16, NOW(), '[{"CourseName":"System Design","Priority":1,"Reason":"Thiết kế"}]', 'Học Design', '[]', 'Gemini', 0),
(14, 16, NOW(), '[{"CourseName":"Testing","Priority":1,"Reason":"QA"}]', 'Học Testing', '[]', 'Gemini', 0),
(15, 16, NOW(), '[{"CourseName":"Physics 2","Priority":1,"Reason":"Tiếp tục"}]', 'Học Physics', '[]', 'Gemini', 0),
(16, 16, NOW(), '[{"CourseName":"Advanced English","Priority":1,"Reason":"Nâng cao"}]', 'Cải thiện English', '[]', 'Gemini', 0),
(17, 16, NOW(), '[{"CourseName":"Computer Vision","Priority":1,"Reason":"AI Vision"}]', 'Học CV', '[]', 'Gemini', 0),
(18, 16, NOW(), '[{"CourseName":"AI Basics","Priority":1,"Reason":"Nền tảng AI"}]', 'Học AI cơ bản', '[]', 'Gemini', 0),
(19, 16, NOW(), '[{"CourseName":"Business English","Priority":1,"Reason":"Thương mại"}]', 'Học Business', '[]', 'Gemini', 0),
(20, 16, NOW(), '[{"CourseName":"TOEIC","Priority":1,"Reason":"Chứng chỉ"}]', 'Lấy TOEIC', '[]', 'Gemini', 0),
(21, 16, NOW(), '[{"CourseName":"NLP","Priority":1,"Reason":"AI Language"}]', 'Học NLP', '[]', 'Gemini', 0),
(22, 16, NOW(), '[{"CourseName":"Data Science","Priority":1,"Reason":"Phân tích"}]', 'Học Data', '[]', 'Gemini', 0),
(23, 16, NOW(), '[{"CourseName":"Sport Management","Priority":1,"Reason":"Quản lý"}]', 'Học Quản lý', '[]', 'Gemini', 0),
(24, 16, NOW(), '[{"CourseName":"Robotics","Priority":1,"Reason":"Robot"}]', 'Học Robot', '[]', 'Gemini', 0),
(25, 16, NOW(), '[{"CourseName":"IoT","Priority":1,"Reason":"Vạn vật"}]', 'Học IoT', '[]', 'Gemini', 0),
(26, 16, NOW(), '[{"CourseName":"Blockchain","Priority":1,"Reason":"Công nghệ mới"}]', 'Học Blockchain', '[]', 'Gemini', 0),
(27, 16, NOW(), '[{"CourseName":"AR/VR","Priority":1,"Reason":"Thực tế ảo"}]', 'Học AR/VR', '[]', 'Gemini', 0),
(28, 16, NOW(), '[{"CourseName":"Game Dev","Priority":1,"Reason":"Game"}]', 'Học Game', '[]', 'Gemini', 0),
(29, 16, NOW(), '[{"CourseName":"Embedded","Priority":1,"Reason":"Nhúng"}]', 'Học Nhúng', '[]', 'Gemini', 0),
(30, 16, NOW(), '[{"CourseName":"Quantum","Priority":1,"Reason":"Lượng tử"}]', 'Học Quantum', '[]', 'Gemini', 0);

-- ============================================
-- HOÀN TẤT!
-- Đã seed 30 records cho mỗi bảng chính
-- ============================================
SELECT 'Seed data completed successfully!' AS Status;
