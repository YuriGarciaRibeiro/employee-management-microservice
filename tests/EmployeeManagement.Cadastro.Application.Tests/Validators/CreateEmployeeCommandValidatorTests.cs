using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Validators;

public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator;

    public CreateEmployeeCommandValidatorTests()
    {
        _validator = new CreateEmployeeCommandValidator();
    }

    [Fact]
    public void Validate_ValidEmployee_ShouldPass()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void Validate_EmptyName_ShouldFail(string? name)
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = name,
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Employee.Name");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldFail()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "Jo",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("mínimo 3 caracteres"));
    }

    [Fact]
    public void Validate_NameTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = new string('A', 101),
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("máximo 100 caracteres"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("abc")]
    public void Validate_InvalidPhone_ShouldFail(string? phone)
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = phone,
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Employee.Phone");
    }

    [Theory]
    [InlineData("+5511999999999")]
    [InlineData("11999999999")]
    [InlineData("+55119999999999")]
    public void Validate_ValidPhone_ShouldPass(string phone)
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = phone,
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_StartDateInPast_ShouldFail()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(-1)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("não pode ser no passado"));
    }

    [Fact]
    public void Validate_StartDateToday_ShouldPass()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.Date
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void Validate_EmptyDepartment_ShouldFail(string? department)
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = department,
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Employee.Department");
    }

    [Fact]
    public void Validate_DepartmentTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateEmployeeCommand(new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = new string('D', 51),
            StartDate = DateTime.UtcNow.AddDays(7)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("máximo 50 caracteres"));
    }
}
