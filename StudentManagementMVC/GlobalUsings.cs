// StudentManagementMVC/GlobalUsings.cs
//
// Global using statements for the MVC project
// Re-export Enums from DataAccess for use in Controllers and Views
// This maintains clean architecture while allowing enum access

global using MajorType = DataAccess.Enums.MajorType;
global using EnrollmentStatus = DataAccess.Enums.EnrollmentStatus;
global using DayOfWeekPair = DataAccess.Enums.DayOfWeekPair;
global using TimeSlot = DataAccess.Enums.TimeSlot;
global using WeekDay = DataAccess.Enums.WeekDay;

// Common Services.Models DTOs used across controllers
global using Services.Models;
