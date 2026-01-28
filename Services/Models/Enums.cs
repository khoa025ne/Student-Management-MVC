// Re-export types from DataAccess layer for use in Controllers via Services
// This follows 3-tier architecture: Controllers -> Services -> DataAccess
// Controllers can access these types through Services.Models namespace

// Global using to make enums accessible - must be at top of file
global using MajorType = DataAccess.Enums.MajorType;
global using EnrollmentStatus = DataAccess.Enums.EnrollmentStatus;
global using DayOfWeekPair = DataAccess.Enums.DayOfWeekPair;
global using TimeSlot = DataAccess.Enums.TimeSlot;
global using WeekDay = DataAccess.Enums.WeekDay;

// Re-export Entity types for Controllers that need them
// This allows Controllers to use Entities without directly referencing DataAccess
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

namespace Services.Models
{
    // Type aliases for DataAccess types - no code duplication
    // Controllers using Services.Models will have access to these types via global using
}
