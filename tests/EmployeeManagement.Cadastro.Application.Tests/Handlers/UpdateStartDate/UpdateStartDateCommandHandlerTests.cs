using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.UpdateStartDate;

public class UpdateStartDateCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<UpdateStartDateCommandHandler>> _loggerMock;
    private readonly UpdateStartDateCommandHandler _handler;

    public UpdateStartDateCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<UpdateStartDateCommandHandler>>();
        _handler = new UpdateStartDateCommandHandler(
            _repositoryMock.Object,
            _eventPublisherMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldUpdateStartDate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow.AddDays(7);
        var newStartDate = DateTime.UtcNow.AddDays(14);

        var employee = new Employee
        {
            Id = employeeId,
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = oldStartDate,
            Status = EmployeeStatus.Inativo
        };

        var command = new UpdateStartDateCommand(employeeId, newStartDate);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        employee.StartDate.Should().Be(newStartDate);
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Employee>(e => e.StartDate == newStartDate)), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var newStartDate = DateTime.UtcNow.AddDays(14);
        var command = new UpdateStartDateCommand(employeeId, newStartDate);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldPublishEvent()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow.AddDays(5);
        var newStartDate = DateTime.UtcNow.AddDays(10);

        var employee = new Employee
        {
            Id = employeeId,
            Name = "Maria Santos",
            Phone = "+5511988888888",
            Department = "RH",
            StartDate = oldStartDate,
            Status = EmployeeStatus.Inativo
        };

        var command = new UpdateStartDateCommand(employeeId, newStartDate);

        _repositoryMock.Setup(x => x.GetByIdAsync(employeeId)).ReturnsAsync(employee);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldSendEmail()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow.AddDays(5);
        var newStartDate = DateTime.UtcNow.AddDays(10);

        var employee = new Employee
        {
            Id = employeeId,
            Name = "Pedro Oliveira",
            Phone = "+5511977777777",
            Department = "Vendas",
            StartDate = oldStartDate,
            Status = EmployeeStatus.Inativo
        };

        var command = new UpdateStartDateCommand(employeeId, newStartDate);

        _repositoryMock.Setup(x => x.GetByIdAsync(employeeId)).ReturnsAsync(employee);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Wait a bit for the Task.Run to execute
        await Task.Delay(100);

        // Assert
        _emailServiceMock.Verify(x => x.SendStartDateUpdatedEmailAsync(
            It.IsAny<string>(),
            employee.Name,
            oldStartDate,
            newStartDate), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow.AddDays(7);
        var newStartDate = DateTime.UtcNow.AddDays(14);

        var employee = new Employee
        {
            Id = employeeId,
            Name = "Ana Costa",
            Phone = "+5511966666666",
            Department = "Marketing",
            StartDate = oldStartDate,
            Status = EmployeeStatus.Inativo,
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var command = new UpdateStartDateCommand(employeeId, newStartDate);

        _repositoryMock.Setup(x => x.GetByIdAsync(employeeId)).ReturnsAsync(employee);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        employee.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }
}
