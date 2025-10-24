using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.UpdateEmployee;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _handler = new UpdateEmployeeCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldUpdateEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var existingEmployee = new Employee
        {
            Id = employeeId,
            Name = "Nome Antigo",
            Phone = "+5511900000000",
            Department = "Dep Antigo",
            StartDate = DateTime.UtcNow.AddDays(5),
            Status = EmployeeStatus.Inativo
        };

        var updateDto = new UpdateEmployeeDto
        {
            Name = "Nome Novo",
            Phone = "+5511999999999",
            Department = "Dep Novo"
        };

        var command = new UpdateEmployeeCommand(employeeId, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(existingEmployee);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingEmployee.Name.Should().Be(updateDto.Name);
        existingEmployee.Phone.Should().Be(updateDto.Phone);
        existingEmployee.Department.Should().Be(updateDto.Department);
        existingEmployee.UpdatedAt.Should().NotBeNull();

        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Employee>(e =>
            e.Id == employeeId &&
            e.Name == updateDto.Name &&
            e.Phone == updateDto.Phone &&
            e.Department == updateDto.Department
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var updateDto = new UpdateEmployeeDto
        {
            Name = "Teste",
            Phone = "+5511999999999",
            Department = "TI"
        };
        var command = new UpdateEmployeeCommand(employeeId, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }
}
