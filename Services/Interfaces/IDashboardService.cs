using Services.Models;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Dashboard Service - Cung cấp thống kê tổng quan
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Lấy tất cả thống kê cho Dashboard
        /// </summary>
        Task<DashboardStatistics> GetDashboardStatisticsAsync();

        /// <summary>
        /// Lấy thống kê theo ngành học cụ thể
        /// </summary>
        Task<DashboardStatistics> GetDashboardStatisticsByMajorAsync(string major);

        /// <summary>
        /// Lấy thống kê theo học kỳ
        /// </summary>
        Task<DashboardStatistics> GetDashboardStatisticsBySemesterAsync(int semesterId);
    }
}
