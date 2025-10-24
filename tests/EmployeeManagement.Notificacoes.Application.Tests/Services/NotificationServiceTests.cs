using EmployeeManagement.Notificacoes.Application.DTOs;
using EmployeeManagement.Notificacoes.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace EmployeeManagement.Notificacoes.Application.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<IHubContext<EmployeeHub, IEmployeeHubClient>> _hubContextMock;
    private readonly Mock<IEmployeeHubClient> _clientProxyMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _hubContextMock = new Mock<IHubContext<EmployeeHub, IEmployeeHubClient>>();
        _clientProxyMock = new Mock<IEmployeeHubClient>();

        // Setup group clients
        var groupManagerMock = new Mock<IGroupManager>();
        _hubContextMock.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);

        _notificationService = new NotificationService(_hubContextMock.Object);
    }

    [Fact]
    public async Task NotifyEmployeeCreatedAsync_ValidData_ShouldSendNotificationToGroup()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "João Silva";
        var department = "TI";
        var startDate = DateTime.UtcNow.AddDays(7);

        _clientProxyMock
            .Setup(x => x.ReceiveEmployeeCreated(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyEmployeeCreatedAsync(employeeId, employeeName, department, startDate);

        // Assert
        _hubContextMock.Verify(x => x.Clients.Group(department), Times.Once);
        _clientProxyMock.Verify(x => x.ReceiveEmployeeCreated(
            It.Is<EmployeeNotificationDto>(n =>
                n.EmployeeId == employeeId &&
                n.EmployeeName == employeeName &&
                n.Department == department &&
                n.StartDate == startDate &&
                n.EventType == "EmployeeCreated" &&
                n.Message.Contains(employeeName) &&
                n.Message.Contains(department)
            )), Times.Once);
    }

    [Fact]
    public async Task NotifyEmployeeActivatedAsync_ValidData_ShouldSendNotificationToGroup()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "Maria Santos";
        var department = "RH";

        _clientProxyMock
            .Setup(x => x.ReceiveEmployeeActivated(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyEmployeeActivatedAsync(employeeId, employeeName, department);

        // Assert
        _hubContextMock.Verify(x => x.Clients.Group(department), Times.Once);
        _clientProxyMock.Verify(x => x.ReceiveEmployeeActivated(
            It.Is<EmployeeNotificationDto>(n =>
                n.EmployeeId == employeeId &&
                n.EmployeeName == employeeName &&
                n.Department == department &&
                n.EventType == "EmployeeActivated" &&
                n.Message.Contains(employeeName) &&
                n.Message.Contains("ativado")
            )), Times.Once);
    }

    [Fact]
    public async Task NotifyStartDateUpdatedAsync_ValidData_ShouldSendNotificationToGroup()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "Pedro Oliveira";
        var department = "Vendas";
        var newStartDate = DateTime.UtcNow.AddDays(14);

        _clientProxyMock
            .Setup(x => x.ReceiveNotification(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyStartDateUpdatedAsync(employeeId, employeeName, department, newStartDate);

        // Assert
        _hubContextMock.Verify(x => x.Clients.Group(department), Times.Once);
        _clientProxyMock.Verify(x => x.ReceiveNotification(
            It.Is<EmployeeNotificationDto>(n =>
                n.EmployeeId == employeeId &&
                n.EmployeeName == employeeName &&
                n.Department == department &&
                n.StartDate == newStartDate &&
                n.EventType == "StartDateUpdated" &&
                n.Message.Contains(employeeName)
            )), Times.Once);
    }

    [Fact]
    public async Task NotifyEmployeeCreatedAsync_DifferentDepartments_ShouldSendToCorrectGroups()
    {
        // Arrange
        var tiEmployee = (Id: Guid.NewGuid(), Name: "Dev 1", Department: "TI", StartDate: DateTime.UtcNow.AddDays(5));
        var rhEmployee = (Id: Guid.NewGuid(), Name: "HR 1", Department: "RH", StartDate: DateTime.UtcNow.AddDays(10));

        _clientProxyMock
            .Setup(x => x.ReceiveEmployeeCreated(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyEmployeeCreatedAsync(tiEmployee.Id, tiEmployee.Name, tiEmployee.Department, tiEmployee.StartDate);
        await _notificationService.NotifyEmployeeCreatedAsync(rhEmployee.Id, rhEmployee.Name, rhEmployee.Department, rhEmployee.StartDate);

        // Assert
        _hubContextMock.Verify(x => x.Clients.Group("TI"), Times.Once);
        _hubContextMock.Verify(x => x.Clients.Group("RH"), Times.Once);
        _clientProxyMock.Verify(x => x.ReceiveEmployeeCreated(It.IsAny<EmployeeNotificationDto>()), Times.Exactly(2));
    }

    [Fact]
    public async Task NotifyEmployeeCreatedAsync_ShouldIncludeTimestamp()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "Test Employee";
        var department = "Test Department";
        var startDate = DateTime.UtcNow.AddDays(7);
        var beforeCall = DateTime.UtcNow;

        _clientProxyMock
            .Setup(x => x.ReceiveEmployeeCreated(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyEmployeeCreatedAsync(employeeId, employeeName, department, startDate);

        var afterCall = DateTime.UtcNow;

        // Assert
        _clientProxyMock.Verify(x => x.ReceiveEmployeeCreated(
            It.Is<EmployeeNotificationDto>(n =>
                n.Timestamp >= beforeCall && n.Timestamp <= afterCall
            )), Times.Once);
    }

    [Fact]
    public async Task NotifyEmployeeActivatedAsync_ShouldNotIncludeStartDate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "Ana Costa";
        var department = "Marketing";

        _clientProxyMock
            .Setup(x => x.ReceiveEmployeeActivated(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyEmployeeActivatedAsync(employeeId, employeeName, department);

        // Assert
        _clientProxyMock.Verify(x => x.ReceiveEmployeeActivated(
            It.Is<EmployeeNotificationDto>(n =>
                n.StartDate == null || !n.StartDate.HasValue
            )), Times.Once);
    }

    [Fact]
    public async Task NotifyStartDateUpdatedAsync_ShouldIncludeFormattedDate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "Carlos Mendes";
        var department = "Financeiro";
        var newStartDate = new DateTime(2025, 12, 15);

        _clientProxyMock
            .Setup(x => x.ReceiveNotification(It.IsAny<EmployeeNotificationDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyStartDateUpdatedAsync(employeeId, employeeName, department, newStartDate);

        // Assert
        _clientProxyMock.Verify(x => x.ReceiveNotification(
            It.Is<EmployeeNotificationDto>(n =>
                n.Message.Contains("15/12/2025")
            )), Times.Once);
    }
}
