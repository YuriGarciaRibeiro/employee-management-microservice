using EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.DeleteEmployee;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _handler = new DeleteEmployeeCommandHandler(_repositoryMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_ShouldDeleteSuccessfully()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            Name = "Test Employee",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(5)
        };

        var command = new DeleteEmployeeCommand(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _repositoryMock
            .Setup(x => x.DeleteAsync(employeeId))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingEmployee_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var command = new DeleteEmployeeCommand(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidDelete_ShouldVerifyEmployeeExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            Name = "Employee to Delete",
            Phone = "+5511988888888",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(10)
        };

        var command = new DeleteEmployeeCommand(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
    }
}
