using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Infrastructure.Data;
using EmployeeManagement.Cadastro.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Repositories;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmployeeRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ValidEmployee_ShouldAddToDatabase()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(employee);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employee.Id);

        var savedEmployee = await _context.Employees.FindAsync(employee.Id);
        savedEmployee.Should().NotBeNull();
        savedEmployee!.Name.Should().Be("João Silva");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEmployee_ShouldReturnEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Maria Santos",
            Phone = "+5511988888888",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(10),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(employee.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employee.Id);
        result.Name.Should().Be("Maria Santos");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEmployee_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployeesOrderedByCreatedAt()
    {
        // Arrange
        var employee1 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 1",
            Phone = "+5511999999991",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(1),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var employee2 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 2",
            Phone = "+5511999999992",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(2),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var employee3 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 3",
            Phone = "+5511999999993",
            Department = "Vendas",
            StartDate = DateTime.UtcNow.AddDays(3),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var employees = result.ToList();
        employees.Should().HaveCount(3);
        employees[0].Name.Should().Be("Emp 3"); // Most recent
        employees[1].Name.Should().Be("Emp 2");
        employees[2].Name.Should().Be("Emp 1"); // Oldest
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnEmployeesInRange()
    {
        // Arrange
        var baseDate = DateTime.UtcNow.Date;

        var employee1 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 1",
            Phone = "+5511999999991",
            Department = "TI",
            StartDate = baseDate.AddDays(5),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        var employee2 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 2",
            Phone = "+5511999999992",
            Department = "RH",
            StartDate = baseDate.AddDays(15),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        var employee3 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emp 3",
            Phone = "+5511999999993",
            Department = "Vendas",
            StartDate = baseDate.AddDays(25),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync(baseDate.AddDays(10), baseDate.AddDays(20));

        // Assert
        var employees = result.ToList();
        employees.Should().HaveCount(1);
        employees[0].Name.Should().Be("Emp 2");
    }

    [Fact]
    public async Task GetGroupedByDepartmentAsync_ShouldGroupByDepartment()
    {
        // Arrange
        var baseDate = DateTime.UtcNow.Date;

        await _context.Employees.AddRangeAsync(
            new Employee { Id = Guid.NewGuid(), Name = "Dev 1", Phone = "+5511999999991", Department = "TI", StartDate = baseDate.AddDays(5), Status = EmployeeStatus.Inativo, CreatedAt = DateTime.UtcNow },
            new Employee { Id = Guid.NewGuid(), Name = "Dev 2", Phone = "+5511999999992", Department = "TI", StartDate = baseDate.AddDays(10), Status = EmployeeStatus.Inativo, CreatedAt = DateTime.UtcNow },
            new Employee { Id = Guid.NewGuid(), Name = "HR 1", Phone = "+5511999999993", Department = "RH", StartDate = baseDate.AddDays(7), Status = EmployeeStatus.Inativo, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetGroupedByDepartmentAsync(baseDate, baseDate.AddDays(30));

        // Assert
        var grouped = result.ToList();
        grouped.Should().HaveCount(2);

        var tiGroup = grouped.First(g => g.Key == "TI");
        tiGroup.Count().Should().Be(2);

        var rhGroup = grouped.First(g => g.Key == "RH");
        rhGroup.Count().Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ExistingEmployee_ShouldUpdateInDatabase()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Pedro Oliveira",
            Phone = "+5511977777777",
            Department = "Vendas",
            StartDate = DateTime.UtcNow.AddDays(7),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        employee.Name = "Pedro Oliveira Silva";
        employee.Department = "Marketing";
        await _repository.UpdateAsync(employee);

        // Assert
        var updatedEmployee = await _context.Employees.FindAsync(employee.Id);
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.Name.Should().Be("Pedro Oliveira Silva");
        updatedEmployee.Department.Should().Be("Marketing");
    }

    [Fact]
    public async Task DeleteAsync_ExistingEmployee_ShouldRemoveFromDatabase()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Ana Costa",
            Phone = "+5511966666666",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(employee.Id);

        // Assert
        var deletedEmployee = await _context.Employees.FindAsync(employee.Id);
        deletedEmployee.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingEmployee_ShouldNotThrow()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var act = async () => await _repository.DeleteAsync(nonExistingId);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
