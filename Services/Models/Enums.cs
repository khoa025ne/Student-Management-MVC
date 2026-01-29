// Services.Models.Enums.cs
// 
// Re-export Enums from DataAccess layer for use in Services and Controllers
// This maintains 3-tier architecture while avoiding duplication
//
// IMPORTANT: This file uses global using to make DataAccess.Enums available
// in the Services namespace. Do NOT define duplicate enums here.

global using MajorType = DataAccess.Enums.MajorType;
global using EnrollmentStatus = DataAccess.Enums.EnrollmentStatus;
global using DayOfWeekPair = DataAccess.Enums.DayOfWeekPair;
global using TimeSlot = DataAccess.Enums.TimeSlot;
global using WeekDay = DataAccess.Enums.WeekDay;

namespace Services.Models
{
    // Enums are re-exported from DataAccess.Enums via global using
    // This allows:
    // - Services to use enums without direct reference to DataAccess
    // - Controllers to use same enums through Services.Models
    // - No duplication or conversion needed
}
