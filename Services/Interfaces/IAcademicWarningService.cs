using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface for Academic Warning Service - Business Logic for academic alerts
    /// </summary>
    public interface IAcademicWarningService
    {
        /// <summary>
        /// Check all students and send academic warnings for GPA/failed courses issues
        /// </summary>
        /// <returns>Number of warnings sent</returns>
        Task<int> CheckAndSendAcademicWarningsAsync();

        /// <summary>
        /// Send GPA warning to a specific student
        /// </summary>
        Task SendGPAWarningAsync(int studentId);

        /// <summary>
        /// Send failed courses warning to a specific student
        /// </summary>
        Task SendFailedCoursesWarningAsync(int studentId, int failedCount);
    }
}
