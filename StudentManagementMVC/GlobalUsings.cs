// Re-export types from Services.Models for use in Controllers
// This follows 3-tier architecture: Controllers -> Services -> DataAccess
// Controllers access Entity types through Services layer

// Re-export Entity types from DataAccess through Services
global using Student = DataAccess.Entities.Student;
global using Class = DataAccess.Entities.Class;
global using Course = DataAccess.Entities.Course;
global using Semester = DataAccess.Entities.Semester;
global using Score = DataAccess.Entities.Score;
global using User = DataAccess.Entities.User;
global using Notification = DataAccess.Entities.Notification;
global using Enrollment = DataAccess.Entities.Enrollment;
global using Role = DataAccess.Entities.Role;
global using AcademicAnalysis = DataAccess.Entities.AcademicAnalysis;
global using LearningPathRecommendation = DataAccess.Entities.LearningPathRecommendation;

// Re-export Enums from DataAccess through Services
global using MajorType = DataAccess.Enums.MajorType;
global using EnrollmentStatus = DataAccess.Enums.EnrollmentStatus;
global using DayOfWeekPair = DataAccess.Enums.DayOfWeekPair;
global using TimeSlot = DataAccess.Enums.TimeSlot;
global using WeekDay = DataAccess.Enums.WeekDay;
