using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_APIs.Controllers;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Service.Utils;

namespace StrateZone.Tests.Controllers_Tests
{
    public class AppointmentControllerTests
    {
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<ILogger<AppointmentController>> _loggerMock;
        private readonly Mock<ScheduleTimeValidator> _scheduleTimeValidatorMock;
        private readonly AppointmentController _controller;

        public AppointmentControllerTests()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _loggerMock = new Mock<ILogger<AppointmentController>>();
            _controller = new AppointmentController(_appointmentServiceMock.Object, _loggerMock.Object, _scheduleTimeValidatorMock.Object);
        }

        [Fact]
        public async Task GetAppointments_ReturnsOkResult_WithAppointments()
        {
            // Arrange
            var parameters = new AppointmentParameters();
            var mockAppointments = new PagedList<AppointmentModel> { new AppointmentModel { AppointmentId = 1 } };
            _appointmentServiceMock.Setup(s => s.GetAppointmentsAsync(parameters)).ReturnsAsync(mockAppointments);

            // Act
            var result = await _controller.GetAppointments(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAppointments_ReturnsOkResult_WhenNoAppointmentsFound()
        {
            _appointmentServiceMock.Setup(s => s.GetAppointmentsAsync(It.IsAny<AppointmentParameters>())).ReturnsAsync(new PagedList<AppointmentModel>());
            var result = await _controller.GetAppointments(new AppointmentParameters());
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("No appointment was found.", okResult.Value);
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsOk_WhenAppointmentExists()
        {
            var appointment = new AppointmentModel { AppointmentId = 1 };
            _appointmentServiceMock.Setup(s => s.GetAppointmentByIdAsync(1)).ReturnsAsync(appointment);
            var result = await _controller.GetAppointmentById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(appointment, okResult.Value);
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            _appointmentServiceMock.Setup(s => s.GetAppointmentByIdAsync(1)).ReturnsAsync((AppointmentModel)null);
            var result = await _controller.GetAppointmentById(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No appointment was found with this ID.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreated_WhenSuccessful()
        {
            var request = new AppointmentRequest()
            {
                UserId = 1,
                TotalPrice = 156_000,
                TablesAppointmentRequests = new()
                {
                    new TablesAppointmentRequest()
                    { 
                        ScheduleTime = DateTime.Parse("2025-04-27T11:30:00"),
                        EndTime = DateTime.Parse("2025-04-27T15:00:00"),
                        Price = 74_000,
                        TableId = 1 
                    },
                    new TablesAppointmentRequest()
                    {
                        ScheduleTime = DateTime.Parse("2025-04-27T14:30:00"),
                        EndTime = DateTime.Parse("2025-04-27T16:00:00"),
                        Price = 52_000,
                        TableId = 2
                    },
                    new TablesAppointmentRequest()
                    {
                        ScheduleTime = DateTime.Parse("2025-04-27T15:00:00"),
                        EndTime = DateTime.Parse("2025-04-27T18:30:00"),
                        Price = 66_000,
                        TableId = 3
                    },
                }
            };
            var appointment = new AppointmentModel() { };
            _appointmentServiceMock.Setup(s => s.CreateAppointmentAsync(request)).ReturnsAsync(appointment);
            var result = await _controller.CreateAppointment(request);
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(appointment, createdResult.Value);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsOk_WhenSuccessful()
        {
            var appointment = new AppointmentModel { AppointmentId = 1 };
            _appointmentServiceMock.Setup(s => s.DeleteAppointmentAsync(1)).ReturnsAsync(appointment);
            var result = await _controller.DeleteAppointment(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(appointment, okResult.Value);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsOk_WhenSuccessful()
        {
            var appointmentModel = new AppointmentModel 
            { 
                AppointmentId = 1,
                UserId = 1,
            };
            _appointmentServiceMock.Setup(s => s.UpdateAppointmentAsync(appointmentModel, 1)).ReturnsAsync(appointmentModel);
            var result = await _controller.UpdateAppointment(appointmentModel, 1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Appointment updated:", okResult.Value.ToString());
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsServerError_WhenExceptionThrown()
        {
            _appointmentServiceMock.Setup(s => s.GetAppointmentByIdAsync(1)).ThrowsAsync(new Exception("Something went wrong"));
            var result = await _controller.GetAppointmentById(1);
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Something went wrong", statusCodeResult.Value.ToString());
        }
    }

}
