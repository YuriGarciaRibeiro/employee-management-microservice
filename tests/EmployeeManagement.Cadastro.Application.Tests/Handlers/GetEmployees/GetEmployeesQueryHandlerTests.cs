using AutoMapper;
using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployees;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.GetEmployees;

public class GetEmployeesQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetEmployeesQueryHandler _handler;

    public GetEmployeesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetEmployeesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPagedResult()
    {
        // Arrange
        var allEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Emp 1", Phone = "+5511999999991", Department = "TI", StartDate = DateTime.UtcNow.AddDays(1) },
            new() { Id = Guid.NewGuid(), Name = "Emp 2", Phone = "+5511999999992", Department = "RH", StartDate = DateTime.UtcNow.AddDays(2) },
            new() { Id = Guid.NewGuid(), Name = "Emp 3", Phone = "+5511999999993", Department = "Vendas", StartDate = DateTime.UtcNow.AddDays(3) }
        };

        var pagedEmployees = allEmployees.Skip(0).Take(10).ToList();

        var employeeDtos = pagedEmployees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            Name = e.Name,
            Phone = e.Phone,
            Department = e.Department,
            StartDate = e.StartDate
        }).ToList();

        var query = new GetEmployeesQuery(1, 10);

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allEmployees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns(employeeDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetEmployeesQuery(1, 10);

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Employee>());

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns(new List<EmployeeDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MultiplePages_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        var allEmployees = new List<Employee>();
        for (int i = 0; i < 23; i++)
        {
            allEmployees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Emp {i + 1}",
                Phone = $"+551199999{i:D4}",
                Department = "TI",
                StartDate = DateTime.UtcNow.AddDays(i + 1)
            });
        }

        var pagedEmployees = allEmployees.Skip(5).Take(5).ToList();

        var employeeDtos = pagedEmployees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            Name = e.Name,
            Phone = e.Phone,
            Department = e.Department,
            StartDate = e.StartDate
        }).ToList();

        var query = new GetEmployeesQuery(2, 5); // Page 2, 5 items per page

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allEmployees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns(employeeDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalPages.Should().Be(5); // 23 items / 5 per page = 5 pages
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(23);
    }

    [Fact]
    public async Task Handle_DifferentPageSizes_ShouldRespectPageSize()
    {
        // Arrange
        var allEmployees = new List<Employee>();
        for (int i = 0; i < 50; i++)
        {
            allEmployees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Emp {i + 1}",
                Phone = $"+551199999{i:D4}",
                Department = "TI",
                StartDate = DateTime.UtcNow.AddDays(i + 1)
            });
        }

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allEmployees);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns<List<Employee>>(employees => employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Phone = e.Phone,
                Department = e.Department,
                StartDate = e.StartDate
            }).ToList());

        // Act
        var query1 = new GetEmployeesQuery(1, 10);
        var result1 = await _handler.Handle(query1, CancellationToken.None);

        var query2 = new GetEmployeesQuery(1, 20);
        var result2 = await _handler.Handle(query2, CancellationToken.None);

        // Assert
        result1.PageSize.Should().Be(10);
        result1.Items.Should().HaveCount(10);
        result1.TotalPages.Should().Be(5);

        result2.PageSize.Should().Be(20);
        result2.Items.Should().HaveCount(20);
        result2.TotalPages.Should().Be(3);
    }
}
