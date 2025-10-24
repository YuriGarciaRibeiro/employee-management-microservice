using AutoMapper;
using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeeById;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.GetEmployeeById;

public class GetEmployeeByIdQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetEmployeeByIdQueryHandler _handler;

    public GetEmployeeByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetEmployeeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_ShouldReturnEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(5),
            Status = EmployeeStatus.Inativo
        };

        var employeeDto = new EmployeeDto
        {
            Id = employeeId,
            Name = employee.Name,
            Phone = employee.Phone,
            Department = employee.Department,
            StartDate = employee.StartDate,
            Status = EmployeeStatus.Inativo
        };

        var query = new GetEmployeeByIdQuery(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _mapperMock
            .Setup(x => x.Map<EmployeeDto>(employee))
            .Returns(employeeDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employeeId);
        result.Name.Should().Be(employee.Name);
        result.Phone.Should().Be(employee.Phone);
        result.Department.Should().Be(employee.Department);

        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _mapperMock.Verify(x => x.Map<EmployeeDto>(employee), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingEmployee_ShouldReturnNull()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var query = new GetEmployeeByIdQuery(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _mapperMock.Verify(x => x.Map<EmployeeDto>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidEmployee_ShouldCallRepositoryOnce()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            Name = "Maria Santos",
            Phone = "+5511988888888",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(10)
        };

        var query = new GetEmployeeByIdQuery(employeeId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _mapperMock
            .Setup(x => x.Map<EmployeeDto>(employee))
            .Returns(new EmployeeDto { Id = employeeId });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
    }
}
