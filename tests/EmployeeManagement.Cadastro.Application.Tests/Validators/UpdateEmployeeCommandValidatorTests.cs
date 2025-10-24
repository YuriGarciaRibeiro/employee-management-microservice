using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Validators;

public class UpdateEmployeeCommandValidatorTests
{
    private readonly UpdateEmployeeCommandValidator _validator;

    public UpdateEmployeeCommandValidatorTests()
    {
        _validator = new UpdateEmployeeCommandValidator();
    }

    [Fact]
    public void Validate_ValidUpdate_ShouldPass()
    {
        // Arrange
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeDto
        {
            Name = "João Silva Atualizado",
            Phone = "+5511999999999",
            Department = "RH"
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
    public void Validate_EmptyName_ShouldFail(string name)
    {
        // Arrange
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeDto
        {
            Name = name,
            Phone = "+5511999999999",
            Department = "TI"
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
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeDto
        {
            Name = "AB",
            Phone = "+5511999999999",
            Department = "TI"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("mínimo 3 caracteres"));
    }

    [Theory]
    [InlineData("+5511999999999")]
    [InlineData("11999999999")]
    public void Validate_ValidPhone_ShouldPass(string phone)
    {
        // Arrange
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeDto
        {
            Name = "João Silva",
            Phone = phone,
            Department = "TI"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void Validate_InvalidPhone_ShouldFail(string phone)
    {
        // Arrange
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeDto
        {
            Name = "João Silva",
            Phone = phone,
            Department = "TI"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
