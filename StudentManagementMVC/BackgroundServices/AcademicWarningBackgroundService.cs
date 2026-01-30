using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Implementations;
using Services.Models;
using Microsoft.AspNetCore.SignalR;
using StudentManagementMVC.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagementMVC.BackgroundServices
{
    /// <summary>
    /// Background Service Host để kiểm tra và gửi cảnh báo học vụ tự động
    /// Đây chỉ là host - business logic nằm ở Services layer (IAcademicWarningService)
    /// </summary>
    public class AcademicWarningBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AcademicWarningBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Kiểm tra mỗi 6 giờ

        public AcademicWarningBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AcademicWarningBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Academic Warning Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunWarningCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in Academic Warning Background Service");
                }

                // Đợi interval trước khi check lại
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("🛑 Academic Warning Background Service stopped");
        }

        private async Task RunWarningCheckAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var warningService = scope.ServiceProvider.GetRequiredService<IAcademicWarningService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            // Cấu hình delegate để gửi SignalR notification
            if (warningService is AcademicWarningService concreteService)
            {
                concreteService.OnSendRealTimeNotification = async (userId, notification) =>
                {
                    await hubContext.Clients.User(userId.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            title = notification.Title,
                            message = notification.Message,
                            type = notification.Type,
                            link = notification.Link,
                            createdAt = notification.CreatedAt
                        });
                };
            }

            // Gọi business logic từ Service layer
            var warningCount = await warningService.CheckAndSendAcademicWarningsAsync();
            _logger.LogInformation($"Academic warning check completed. {warningCount} warnings sent.");
        }
    }
}
