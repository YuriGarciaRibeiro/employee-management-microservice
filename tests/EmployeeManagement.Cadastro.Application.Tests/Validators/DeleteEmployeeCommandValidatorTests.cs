using EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Validators;

public class DeleteEmployeeCommandValidatorTests
{
    private readonly DeleteEmployeeCommandValidator _validator;

    public DeleteEmployeeCommandValidatorTests()
    {
        _validator = new DeleteEmployeeCommandValidator();
    }

    [Fact]
    public void Validate_ValidId_ShouldPass()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyGuid_ShouldFail()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().Be("Id");
        result.Errors[0].ErrorMessage.Should().Be("ID do funcionário é obrigatório");
    }

    [Fact]
    public void Validate_MultipleValidIds_ShouldAllPass()
    {
        // Arrange
        var validIds = new[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        foreach (var id in validIds)
        {
            var command = new DeleteEmployeeCommand(id);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
