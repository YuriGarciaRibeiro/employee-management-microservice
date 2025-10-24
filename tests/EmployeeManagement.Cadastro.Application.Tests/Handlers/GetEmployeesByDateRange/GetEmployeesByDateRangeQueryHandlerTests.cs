using AutoMapper;
using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesByDateRange;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.GetEmployeesByDateRange;

public class GetEmployeesByDateRangeQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetEmployeesByDateRangeQueryHandler _handler;

    public GetEmployeesByDateRangeQueryHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetEmployeesByDateRangeQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidDateRange_ShouldReturnEmployees()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var employees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Emp 1", Phone = "+5511999999991", Department = "TI", StartDate = startDate.AddDays(5) },
            new() { Id = Guid.NewGuid(), Name = "Emp 2", Phone = "+5511999999992", Department = "RH", StartDate = startDate.AddDays(10) },
            new() { Id = Guid.NewGuid(), Name = "Emp 3", Phone = "+5511999999993", Department = "Vendas", StartDate = startDate.AddDays(20) }
        };

        var employeeDtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            Name = e.Name,
            Phone = e.Phone,
            Department = e.Department,
            StartDate = e.StartDate
        }).ToList();

        var query = new GetEmployeesByDateRangeQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(employees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(employees))
            .Returns(employeeDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(employeeDtos);

        _repositoryMock.Verify(x => x.GetByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task Handle_NoEmployeesInRange_ShouldReturnEmptyList()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var query = new GetEmployeesByDateRangeQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(new List<Employee>());

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns(new List<EmployeeDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_SingleDayRange_ShouldReturnMatchingEmployees()
    {
        // Arrange
        var targetDate = DateTime.UtcNow.AddDays(10).Date;

        var employees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Emp 1", Phone = "+5511999999991", Department = "TI", StartDate = targetDate }
        };

        var employeeDtos = new List<EmployeeDto>
        {
            new() { Id = employees[0].Id, Name = employees[0].Name, Phone = employees[0].Phone, Department = employees[0].Department, StartDate = targetDate }
        };

        var query = new GetEmployeesByDateRangeQuery(targetDate, targetDate);

        _repositoryMock
            .Setup(x => x.GetByDateRangeAsync(targetDate, targetDate))
            .ReturnsAsync(employees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(employees))
            .Returns(employeeDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].StartDate.Should().Be(targetDate);
    }

    [Fact]
    public async Task Handle_LargeRange_ShouldReturnAllMatchingEmployees()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(365).Date;

        var employees = new List<Employee>();
        for (int i = 0; i < 50; i++)
        {
            employees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Employee {i}",
                Phone = $"+551199999{i:D4}",
                Department = i % 2 == 0 ? "TI" : "RH",
                StartDate = startDate.AddDays(i * 7)
            });
        }

        var employeeDtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            Name = e.Name,
            Phone = e.Phone,
            Department = e.Department,
            StartDate = e.StartDate
        }).ToList();

        var query = new GetEmployeesByDateRangeQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(employees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(employees))
            .Returns(employeeDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(50);
    }
}
