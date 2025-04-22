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
        private Timer? _topContributorAssignTimer = null;

        private static readonly TimeSpan _userCleanupInterval = TimeSpan.FromHours(12);
        private static readonly TimeSpan _appointmentRequestsCleanupInterval = TimeSpan.FromSeconds(120);
        private static readonly TimeSpan _tablesAppointmentsCleanupInterval = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan _appointmentsUpdateInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan _topContributorAssignInterval = TimeSpan.FromDays(7);

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
            _topContributorAssignTimer = new Timer(AssignTopContributorsAfterEvery7Days, null, TimeSpan.Zero, _topContributorAssignInterval);

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

        private void AssignTopContributorsAfterEvery7Days(object? state)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await userService.AssignTopContributorsAsync();
                    }

                    _logger.LogInformation($"IUserService executed: Assigned label for top contributors.");
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

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var tablesAppointmentService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                        var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        var toBeAnnouncedTablesAppointments = await tablesAppointmentService.GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests();

                        var notifications = new List<NotificationRequest>();
                        foreach (var tableAppointment in toBeAnnouncedTablesAppointments)
                        {
                            var appointment = await appointmentService.GetAppointmentByIdAsync((int)tableAppointment.AppointmentId);
                            var userId = appointment.UserId;

                            string timeString = $"ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}, từ {tableAppointment.ScheduleTime.TimeOfDay} đến {tableAppointment.EndTime.TimeOfDay}";

                            NotificationRequest notif = new()
                            {
                                ToUser = userId,
                                Title = $"Các lời mời chơi cờ mà bạn đã gửi cho bàn {tableAppointment.TableId} đã bị từ chối!",
                                Content = $"Toàn bộ các lời mời chơi cờ mà bạn đã gửi cho bàn {tableAppointment.TableId}, vào {timeString} (mã đơn #{tableAppointment.AppointmentId}) " +
                                $"đều đã bị từ chối hoặc đã hết hạn. Bấm để xem chi tiết.",
                                TablesAppointmentId = tableAppointment.Id,
                                Type = StrateZone_Repository.Parameters.PostgreEnums.NotificationType.tables_appointment_invitations_timedout
                            };

                            notifications.Add(notif);
                        }

                        await notificationService.CreateNotificationsForRejectedTablesAppoimentsAsync(notifications);

                        _logger.LogInformation($"ITablesAppointmentService executed: Sent notifications to " +
                            $"{toBeAnnouncedTablesAppointments.Count} tables on appointment(s).");
                    }
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

            if (_topContributorAssignTimer != null)
            {
                await _topContributorAssignTimer.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _userServiceTimer?.Dispose();
            _appointmentRequestsServiceTimer?.Dispose();
            _tablesAppointmentsServiceTimer?.Dispose();
            _appointmentServiceTimer?.Dispose();
            _topContributorAssignTimer?.Dispose();
        }
    }
}
