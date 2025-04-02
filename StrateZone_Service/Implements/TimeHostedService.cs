using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer? _userServiceTimer = null;
        private Timer? _appointmentRequestsServiceTimer = null;
        private Timer? _tablesAppointmentsServiceTimer = null;
        private Timer? _timer = null;

        private static readonly TimeSpan _userCleanupInterval = TimeSpan.FromHours(12);
        private static readonly TimeSpan _appointmentRequestsCleanupInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan _tablesAppointmentsCleanupInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan _generalWorkInterval = TimeSpan.FromSeconds(15);

        public TimedHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<TimedHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _userServiceTimer = new Timer(DeleteUnactivatedAccounts, null, TimeSpan.Zero, _userCleanupInterval);
            _appointmentRequestsServiceTimer = new Timer(UpdateExpiredAppointmentRequestStatus, null, TimeSpan.Zero, _appointmentRequestsCleanupInterval);
            _tablesAppointmentsServiceTimer = new Timer(UpdateStatusForExpiredAndIncomingTablesAppointments, null, TimeSpan.Zero, _tablesAppointmentsCleanupInterval);
            _timer = new Timer(DoWork, null, TimeSpan.Zero, _generalWorkInterval);

            return Task.CompletedTask;
        }

        private void DeleteUnactivatedAccounts(object? state)
        {
            Task.Run(async () =>
            {
                try
                {
                    int count = 0;

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        count = await userService.DeleteUnactivatedAccountsAsync(3);
                    }

                    _logger.LogInformation($"IUserService cleanup executed: Deleted {count} unactivated account(s).");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting unactivated accounts.");
                }
            });
        }

        private void SendAppointmentNotifications(object? state)
        {

        }

        private void UpdateStatusForExpiredAndIncomingTablesAppointments(object? state)
        {
            Task.Run(async () =>
            {
                try
                {
                    int count = 0;

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var tablesAppointmentsService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                        count = await tablesAppointmentsService.UpdateStatusForExpiredAndIncomingTablesAppointments();
                    }

                    _logger.LogInformation($"ITablesAppointmentsService cleanup executed: Changed status for {count} expired request(s).");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating tables appoointments status.");
                }
            });
        }

        private void UpdateExpiredAppointmentRequestStatus(object? data)
        {
            Task.Run(async () =>
            {
                try
                {
                    int count = 0;

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var appointmentRequestsService = scope.ServiceProvider.GetRequiredService<IAppointmentrequestService>();
                        count = await appointmentRequestsService.UpdateExpiredAppointmentRequests();
                    }
                    
                    _logger.LogInformation($"IAppointmentrequestService cleanup executed: Changed status for {count} expired request(s).");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating expired appointment requests status.");
                }
            });
        }

        private void DoWork(object? state)
        {
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _userServiceTimer?.Change(Timeout.Infinite, 0);
            _appointmentRequestsServiceTimer?.Change(Timeout.Infinite, 0);
            _timer?.Change(Timeout.Infinite, 0);

            if (_userServiceTimer != null)
            {
                await _userServiceTimer.DisposeAsync();
            }

            if (_appointmentRequestsServiceTimer != null)
            {
                await _appointmentRequestsServiceTimer.DisposeAsync();
            }

            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _userServiceTimer?.Dispose();
            _appointmentRequestsServiceTimer?.Dispose();
            _timer?.Dispose();
        }
    }
}
