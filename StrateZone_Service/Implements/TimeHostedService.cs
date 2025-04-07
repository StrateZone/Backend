using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StrateZone_Service.CustomModels.RequestModels;
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
        private Timer? _appointmentServiceTimer = null;

        private static readonly TimeSpan _userCleanupInterval = TimeSpan.FromHours(12);
        private static readonly TimeSpan _appointmentRequestsCleanupInterval = TimeSpan.FromSeconds(120);
        private static readonly TimeSpan _tablesAppointmentsCleanupInterval = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan _appointmentsUpdateInterval = TimeSpan.FromSeconds(30);

        public TimedHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<TimedHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _userServiceTimer = new Timer(DeleteUnactivatedAccounts, null, TimeSpan.Zero, _userCleanupInterval);
            _appointmentRequestsServiceTimer = new Timer(UpdateExpiredAppointmentRequestStatus, null, new TimeSpan(0, 0, 5), _appointmentRequestsCleanupInterval);
            _tablesAppointmentsServiceTimer = new Timer(UpdateStatusForExpiredAndIncomingTablesAppointments, null, new TimeSpan(0, 0, 10), _tablesAppointmentsCleanupInterval);
            _appointmentServiceTimer = new Timer(UpdateStatusForAppointmentBasedOnTablesAppointments, null, new TimeSpan(0, 0, 15), _appointmentsUpdateInterval);

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

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var tablesAppointmentService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                        var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        var toBeCancelledTablesAppointments = await tablesAppointmentService.GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests();
                        foreach ( var table in toBeCancelledTablesAppointments )
                        {
                            var appointment = await appointmentService.GetAppointmentByIdAsync((int)table.AppointmentId);
                            var userId = appointment.UserId;

                            await tablesAppointmentService.CancelTablesAppointment(table.Id, userId);

                            NotificationRequest notif = new()
                            {
                                ToUser = userId,
                                Title = $"Your table has been automatically cancelled!",
                                Content = $"Your appointment on table {table.TableId}, appointment ID {table.AppointmentId} has been automatically cancelled and refunded. " +
                                $"Reason: All of the sent invitations to this table have been either rejected or cancelled.",
                                TablesAppointmentId = table.Id,
                            };

                            await notificationService.CreateNotificationAsync(notif);
                        }

                        _logger.LogInformation($"ITablesAppointmentService executed: Cancelled and refunded for " +
                            $"{toBeCancelledTablesAppointments.Count} tables on appointment(s).");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating expired appointment requests status.");
                }
            });
        }

        private void UpdateStatusForAppointmentBasedOnTablesAppointments(object? state)
        {
            Task.Run(async () =>
            {
                try
                {
                    int count = 0;

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var appointmentRequestsService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                        count = await appointmentRequestsService.UpdateStatusForAppointmentBasedOnTablesAppointments();
                    }

                    _logger.LogInformation($"IAppointmentService auto updater executed: Changed status for {count} expired request(s).");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating appointment status.");
                }
            });
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _userServiceTimer?.Change(Timeout.Infinite, 0);
            _appointmentRequestsServiceTimer?.Change(Timeout.Infinite, 0);
            _tablesAppointmentsServiceTimer?.Change(Timeout.Infinite, 0);
            _appointmentServiceTimer?.Change(Timeout.Infinite, 0);

            if (_userServiceTimer != null)
            {
                await _userServiceTimer.DisposeAsync();
            }

            if (_appointmentRequestsServiceTimer != null)
            {
                await _appointmentRequestsServiceTimer.DisposeAsync();
            }

            if (_tablesAppointmentsServiceTimer != null)
            {
                await _tablesAppointmentsServiceTimer.DisposeAsync();
            }

            if (_appointmentServiceTimer != null)
            {
                await _appointmentServiceTimer.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _userServiceTimer?.Dispose();
            _appointmentRequestsServiceTimer?.Dispose();
            _tablesAppointmentsServiceTimer?.Dispose();
            _appointmentServiceTimer?.Dispose();
        }
    }
}
