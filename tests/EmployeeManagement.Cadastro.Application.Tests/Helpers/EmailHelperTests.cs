using EmployeeManagement.Cadastro.Application.Helpers;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Helpers;

public class EmailHelperTests
{
    [Fact]
    public void GenerateCompanyEmail_WithValidName_ShouldReturnFormattedEmail()
    {
        // Arrange
        var employeeName = "João Silva";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("joão.silva@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithMultipleSpaces_ShouldReplaceAllSpaces()
    {
        // Arrange
        var employeeName = "Maria da Silva Santos";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("maria.da.silva.santos@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithUppercaseName_ShouldConvertToLowercase()
    {
        // Arrange
        var employeeName = "PEDRO OLIVEIRA";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("pedro.oliveira@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithMixedCaseName_ShouldConvertToLowercase()
    {
        // Arrange
        var employeeName = "AnA PaUlA";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("ana.paula@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithCustomDomain_ShouldUseCustomDomain()
    {
        // Arrange
        var employeeName = "Carlos Santos";
        var customDomain = "minhaempresa.com.br";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName, customDomain);

        // Assert
        result.Should().Be("carlos.santos@minhaempresa.com.br");
    }

    [Fact]
    public void GenerateCompanyEmail_WithSingleName_ShouldReturnValidEmail()
    {
        // Arrange
        var employeeName = "João";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("joão@empresa.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateCompanyEmail_WithEmptyOrWhitespaceName_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        Action act = () => EmailHelper.GenerateCompanyEmail(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Nome do funcionário não pode ser vazio*")
            .And.ParamName.Should().Be("employeeName");
    }

    [Fact]
    public void GenerateCompanyEmail_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        string? nullName = null;

        // Act
        Action act = () => EmailHelper.GenerateCompanyEmail(nullName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Nome do funcionário não pode ser vazio*")
            .And.ParamName.Should().Be("employeeName");
    }

    [Fact]
    public void GenerateCompanyEmail_WithSpecialCharacters_ShouldKeepSpecialCharacters()
    {
        // Arrange
        var employeeName = "José D'Angelo";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("josé.d'angelo@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithAccentedCharacters_ShouldKeepAccents()
    {
        // Arrange
        var employeeName = "André Müller";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().Be("andré.müller@empresa.com");
    }

    [Fact]
    public void GenerateCompanyEmail_WithDefaultDomain_ShouldUseEmpresaDotCom()
    {
        // Arrange
        var employeeName = "Test User";

        // Act
        var result = EmailHelper.GenerateCompanyEmail(employeeName);

        // Assert
        result.Should().EndWith("@empresa.com");
    }
}
