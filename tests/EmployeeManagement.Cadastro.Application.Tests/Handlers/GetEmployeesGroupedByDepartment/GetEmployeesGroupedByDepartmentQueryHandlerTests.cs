using AutoMapper;
using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesGroupedByDepartment;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.GetEmployeesGroupedByDepartment;

public class GetEmployeesGroupedByDepartmentQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetEmployeesGroupedByDepartmentQueryHandler _handler;

    public GetEmployeesGroupedByDepartmentQueryHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetEmployeesGroupedByDepartmentQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnGroupedEmployees()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var tiEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Dev 1", Phone = "+5511999999991", Department = "TI", StartDate = startDate.AddDays(5) },
            new() { Id = Guid.NewGuid(), Name = "Dev 2", Phone = "+5511999999992", Department = "TI", StartDate = startDate.AddDays(10) }
        };

        var rhEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "HR 1", Phone = "+5511999999993", Department = "RH", StartDate = startDate.AddDays(7) }
        };

        var grouped = new List<IGrouping<string, Employee>>
        {
            new TestGrouping<string, Employee>("TI", tiEmployees),
            new TestGrouping<string, Employee>("RH", rhEmployees)
        };

        var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetGroupedByDepartmentAsync(startDate, endDate))
            .ReturnsAsync(grouped);

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
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var tiGroup = result.First(g => g.Department == "TI");
        tiGroup.EmployeeCount.Should().Be(2);
        tiGroup.Employees.Should().HaveCount(2);

        var rhGroup = result.First(g => g.Department == "RH");
        rhGroup.EmployeeCount.Should().Be(1);
        rhGroup.Employees.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_NoEmployees_ShouldReturnEmptyList()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetGroupedByDepartmentAsync(startDate, endDate))
            .ReturnsAsync(new List<IGrouping<string, Employee>>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_SingleDepartment_ShouldReturnOneGroup()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var employees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Emp 1", Phone = "+5511999999991", Department = "Vendas", StartDate = startDate.AddDays(5) },
            new() { Id = Guid.NewGuid(), Name = "Emp 2", Phone = "+5511999999992", Department = "Vendas", StartDate = startDate.AddDays(10) },
            new() { Id = Guid.NewGuid(), Name = "Emp 3", Phone = "+5511999999993", Department = "Vendas", StartDate = startDate.AddDays(15) }
        };

        var grouped = new List<IGrouping<string, Employee>>
        {
            new TestGrouping<string, Employee>("Vendas", employees)
        };

        var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetGroupedByDepartmentAsync(startDate, endDate))
            .ReturnsAsync(grouped);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns<List<Employee>>(emps => emps.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Phone = e.Phone,
                Department = e.Department,
                StartDate = e.StartDate
            }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Department.Should().Be("Vendas");
        result[0].EmployeeCount.Should().Be(3);
        result[0].Employees.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_MultipleDepartments_ShouldReturnAllGroups()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.AddDays(30).Date;

        var tiEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Dev 1", Phone = "+5511999999991", Department = "TI", StartDate = startDate.AddDays(5) }
        };

        var rhEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "HR 1", Phone = "+5511999999992", Department = "RH", StartDate = startDate.AddDays(7) }
        };

        var vendasEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Sales 1", Phone = "+5511999999993", Department = "Vendas", StartDate = startDate.AddDays(10) }
        };

        var marketingEmployees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Marketing 1", Phone = "+5511999999994", Department = "Marketing", StartDate = startDate.AddDays(12) }
        };

        var grouped = new List<IGrouping<string, Employee>>
        {
            new TestGrouping<string, Employee>("TI", tiEmployees),
            new TestGrouping<string, Employee>("RH", rhEmployees),
            new TestGrouping<string, Employee>("Vendas", vendasEmployees),
            new TestGrouping<string, Employee>("Marketing", marketingEmployees)
        };

        var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);

        _repositoryMock
            .Setup(x => x.GetGroupedByDepartmentAsync(startDate, endDate))
            .ReturnsAsync(grouped);

        _mapperMock
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns<List<Employee>>(emps => emps.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Phone = e.Phone,
                Department = e.Department,
                StartDate = e.StartDate
            }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(4);
        result.Select(g => g.Department).Should().Contain(new[] { "TI", "RH", "Vendas", "Marketing" });
        result.All(g => g.EmployeeCount == 1).Should().BeTrue();
    }

    // Helper class to create IGrouping for testing
    private class TestGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _elements;

        public TestGrouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
