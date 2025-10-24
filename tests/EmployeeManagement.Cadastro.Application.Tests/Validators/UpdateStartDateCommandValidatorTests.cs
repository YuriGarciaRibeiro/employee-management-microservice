using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Validators;

public class UpdateStartDateCommandValidatorTests
{
    private readonly UpdateStartDateCommandValidator _validator;

    public UpdateStartDateCommandValidatorTests()
    {
        _validator = new UpdateStartDateCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyEmployeeId_ShouldFail()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.Empty,
            DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "EmployeeId");
        result.Errors.Should().Contain(e => e.ErrorMessage == "ID do funcionário é obrigatório");
    }

    [Fact]
    public void Validate_PastStartDate_ShouldFail()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "NewStartDate");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Data de início não pode ser no passado");
    }

    [Fact]
    public void Validate_TodayAsStartDate_ShouldPass()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_FutureStartDate_ShouldPass()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_FarFutureStartDate_ShouldPass()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddYears(1)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_BothInvalid_ShouldReturnMultipleErrors()
    {
        // Arrange
        var command = new UpdateStartDateCommand(
            Guid.Empty,
            DateTime.UtcNow.AddDays(-10)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "EmployeeId");
        result.Errors.Should().Contain(e => e.PropertyName == "NewStartDate");
    }
}
