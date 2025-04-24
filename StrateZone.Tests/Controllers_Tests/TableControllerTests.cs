using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using static StrateZone_Repository.Parameters.PostgreEnums;
using StrateZone_APIs.Controllers;
using StrateZone_Repository.Parameters;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Pagination;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone.Tests.Controllers_Tests
{
    public class TableControllerTests
    {
        private readonly Mock<ITableService> _tableServiceMock;
        private readonly Mock<ILogger<TableController>> _loggerMock;
        private readonly TableController _controller;

        public TableControllerTests()
        {
            _tableServiceMock = new Mock<ITableService>();
            _loggerMock = new Mock<ILogger<TableController>>();
            _controller = new TableController(_tableServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAvailableTablesByGameType_ShouldReturnOkResult_WithTables()
        {
            // Arrange
            var tables = new List<TableResponse> { new TableResponse { TableId = 1 } };
            var pagedResponse = new PagedListResponse<TableResponse>(new PagedList<TableResponse>(tables, tables.Count, 1, 10));

            _tableServiceMock.Setup(s => s.GetAvailableTablesByGameTypeAsync(It.IsAny<TableParameters>(), It.IsAny<GameTypeEnum>()))
                             .ReturnsAsync(pagedResponse.PagedList);

            // Act
            var result = await _controller.GetAvailableTablesByGameType(
                new TableParameters()
                {
                    StartTime = DateTime.Parse("2025-06-25T08:00:00"),
                    EndTime = DateTime.Parse("2025-06-25T12:00:00"),
                },
                GameTypeEnum.chess);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<PagedListResponse<TableResponse>>().Subject;

            response.TotalCount.Should().Be(1);
            response.PagedList.Should().HaveCount(1);
            response.PagedList[0].TableId.Should().Be(1);
        }

        [Theory]
        [InlineData("2025-06-25T07:30:00", "2025-06-25T12:00:00", "Appointment must be scheduled between 8AM and 10PM.")]
        [InlineData("2025-06-25T08:30:00", "2025-06-25T08:30:00", "The minimum duration between start and end time is 30 minutes.")]
        [InlineData("2025-06-25T10:30:00", "2025-06-26T12:00:00", "Start time and End time must be within the same day.")]
        [InlineData("2025-06-25T10:30:00", "2025-06-25T09:00:00", "Start time must be earlier than End time.")]
        [InlineData("2025-06-25T10:45:00", "2025-06-25T12:00:00", "Appointment's schedule and end time's minute parts must be divisible by 30.")]
        public async Task GetAvailableTablesByGameType_ShouldReturnBadRequest_WhenTimeParametersAreInvalid(string StartTime, string EndTime, string errorMessage)
        {
            // Arrange
            var parameters = new TableParameters()
            {
                StartTime = DateTime.Parse(StartTime),
                EndTime = DateTime.Parse(EndTime),
            };

            _tableServiceMock.Setup(s => s.GetAvailableTablesByGameTypeAsync(It.IsAny<TableParameters>(), It.IsAny<GameTypeEnum>()))
                             .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.GetAvailableTablesByGameType(parameters, GameTypeEnum.chess);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var responseMessage = badRequestResult.Value.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null) as string;

            responseMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task GetAvailableTablesByGameTypeAndRoomType_ShouldReturnOkResult_WithTables()
        {
            // Arrange
            var tables = new List<TableResponse> 
            { 
                new TableResponse ()
                { 
                    TableId = 1,
                    RoomId = 1,
                    RoomName = "BA001",
                    RoomDescription = "- Giá cả phải chăng.\\n- Nhiều bàn.",
                    RoomType = "basic",
                    GameTypeId = 1,
                },
            };
            var pagedResponse = new PagedListResponse<TableResponse>(new PagedList<TableResponse>(tables, tables.Count, 1, 10));

            _tableServiceMock.Setup(s => s.GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(
                It.IsAny<TableParameters>(), It.IsAny<GameTypeEnum[]>(), It.IsAny<RoomType[]>()
            )).ReturnsAsync(pagedResponse.PagedList);

            // Act
            var result = await _controller.GetAvailableTablesByGameTypeAndRoomType(
                new TableParameters()
                {
                    StartTime = DateTime.Parse("2035-06-25T08:00:00"),
                    EndTime = DateTime.Parse("2035-06-25T12:00:00"),
                },
                new[] { GameTypeEnum.chess, GameTypeEnum.xiangqi },
                new[] { RoomType.premium, RoomType.basic, RoomType.openspaced }
            );

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<PagedListResponse<TableResponse>>().Subject;

            response.TotalCount.Should().Be(1);
            response.PagedList.Should().HaveCount(1);
            response.PagedList[0].TableId.Should().Be(1);
        }

        [Theory]
        [InlineData("2025-06-25T07:30:00", "2025-06-25T12:00:00", "Appointment must be scheduled between 8AM and 10PM.")]
        [InlineData("2025-06-25T08:30:00", "2025-06-25T08:30:00", "The minimum duration between start and end time is 30 minutes.")]
        [InlineData("2025-06-25T10:30:00", "2025-06-26T12:00:00", "Start time and End time must be within the same day.")]
        [InlineData("2025-06-25T10:30:00", "2025-06-25T09:00:00", "Start time must be earlier than End time.")]
        [InlineData("2025-06-25T10:45:00", "2025-06-25T12:00:00", "Appointment's schedule and end time's minute parts must be divisible by 30.")]
        public async Task GetAvailableTablesByGameTypeAndRoomType_ShouldReturnBadRequest_WhenInvalidParameters(string startTime, string endTime, string expectedErrorMessage)
        {
            // Arrange
            var parameters = new TableParameters()
            {
                StartTime = DateTime.Parse(startTime),
                EndTime = DateTime.Parse(endTime),
            };

            _tableServiceMock.Setup(s => s.GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(
                It.IsAny<TableParameters>(), It.IsAny<GameTypeEnum[]>(), It.IsAny<RoomType[]>()
            )).ThrowsAsync(new ArgumentException(expectedErrorMessage));

            // Act
            var result = await _controller.GetAvailableTablesByGameTypeAndRoomType(parameters, new[] { GameTypeEnum.chess, GameTypeEnum.xiangqi }, new[] { RoomType.premium, RoomType.basic, RoomType.openspaced });

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var responseMessage = badRequestResult.Value.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null) as string;

            responseMessage.Should().Be(expectedErrorMessage);
        }


        [Fact]
        public async Task CreateTable_ShouldReturnCreatedResult_WhenTableIsCreated()
        {
            // Arrange
            var request = new TableRequest { Room_Id = 1, GameType_Id = 1 };
            var tableResponse = new TableModel { TableId = 1 };

            _tableServiceMock.Setup(s => s.CreateTableAsync(request))
                             .ReturnsAsync(tableResponse);

            // Act
            var result = await _controller.CreateTable(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
            createdResult.Location.Should().Be("Table created");
            createdResult.Value.Should().BeEquivalentTo(tableResponse);
        }

        [Fact]
        public async Task CreateTable_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new TableRequest { Room_Id = -1 };

            _tableServiceMock.Setup(s => s.CreateTableAsync(request))
                             .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreateTable(request);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().BeEquivalentTo(new { message = "Internal server error" });
        }

        [Fact]
        public async Task UpdateTable_ShouldReturnOkResult_WhenTableIsUpdated()
        {
            // Arrange
            var tableModel = new TableModel { RoomId = 1, GameTypeId = 2 };
            var tableResponse = new TableModel { TableId = 1 };

            _tableServiceMock.Setup(s => s.UpdateTableAsync(tableModel, 1))
                             .ReturnsAsync(tableResponse);

            // Act
            var result = await _controller.UpdateTable(tableModel, 1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(tableResponse);
        }

        [Fact]
        public async Task UpdateTable_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var tableModel = new TableModel { RoomId = 1, GameTypeId = -1 };

            _tableServiceMock.Setup(s => s.UpdateTableAsync(tableModel, 1))
                             .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateTable(tableModel, 1);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().BeEquivalentTo(new { message = "Internal server error" });
        }

        [Fact]
        public async Task DeleteTable_ShouldReturnOkResult_WhenTableIsDeleted()
        {
            // Arrange
            var tableResponse = new TableModel { TableId = 1 };

            _tableServiceMock.Setup(s => s.DeleteTableAsync(1))
                             .ReturnsAsync(tableResponse);

            // Act
            var result = await _controller.DeleteTable(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(tableResponse);
        }

        [Fact]
        public async Task DeleteTable_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _tableServiceMock.Setup(s => s.DeleteTableAsync(1))
                             .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteTable(1);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().BeEquivalentTo(new { message = "Internal server error" });
        }
    }
}