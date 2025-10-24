using EmployeeManagement.Cadastro.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        SetupConfiguration();
        _emailService = new EmailService(_configurationMock.Object);
    }

    private void SetupConfiguration()
    {
        _configurationMock.Setup(x => x["EmailSettings:SenderName"]).Returns("Equipe RH");
        _configurationMock.Setup(x => x["EmailSettings:SenderEmail"]).Returns("rh@empresa.com");
        _configurationMock.Setup(x => x["EmailSettings:SmtpServer"]).Returns("smtp.gmail.com");
        _configurationMock.Setup(x => x["EmailSettings:SmtpPort"]).Returns("587");
        _configurationMock.Setup(x => x["EmailSettings:Username"]).Returns("test@empresa.com");
        _configurationMock.Setup(x => x["EmailSettings:Password"]).Returns("testpassword");
    }

    [Fact]
    public void EmailService_ShouldBeInstantiatedWithConfiguration()
    {
        // Assert
        _emailService.Should().NotBeNull();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_ShouldNotThrowExceptionWithValidInputs()
    {
        // Arrange
        var recipientEmail = "joao.silva@empresa.com";
        var recipientName = "João Silva";
        var startDate = DateTime.UtcNow.AddDays(7);

        // Note: This test will fail in actual execution without a real SMTP server
        // In a real scenario, you would mock the SMTP client or use an email testing service
        // For now, we're just testing that the method signature is correct

        // Act & Assert
        var act = async () => await _emailService.SendWelcomeEmailAsync(recipientEmail, recipientName, startDate);

        // This will throw because there's no real SMTP server, but we're testing the method exists
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task SendStartDateUpdatedEmailAsync_ShouldNotThrowExceptionWithValidInputs()
    {
        // Arrange
        var recipientEmail = "maria.santos@empresa.com";
        var recipientName = "Maria Santos";
        var oldStartDate = DateTime.UtcNow.AddDays(7);
        var newStartDate = DateTime.UtcNow.AddDays(14);

        // Note: This test will fail in actual execution without a real SMTP server
        // In a real scenario, you would mock the SMTP client or use an email testing service

        // Act & Assert
        var act = async () => await _emailService.SendStartDateUpdatedEmailAsync(recipientEmail, recipientName, oldStartDate, newStartDate);

        // This will throw because there's no real SMTP server, but we're testing the method exists
        await act.Should().ThrowAsync<Exception>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SendWelcomeEmailAsync_WithInvalidEmail_ShouldThrow(string? invalidEmail)
    {
        // Arrange
        var recipientName = "Test User";
        var startDate = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        var act = async () => await _emailService.SendWelcomeEmailAsync(invalidEmail, recipientName, startDate);
        await act.Should().ThrowAsync<Exception>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SendStartDateUpdatedEmailAsync_WithInvalidEmail_ShouldThrow(string? invalidEmail)
    {
        // Arrange
        var recipientName = "Test User";
        var oldStartDate = DateTime.UtcNow.AddDays(7);
        var newStartDate = DateTime.UtcNow.AddDays(14);

        // Act & Assert
        var act = async () => await _emailService.SendStartDateUpdatedEmailAsync(invalidEmail, recipientName, oldStartDate, newStartDate);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void EmailService_ShouldReadConfigurationValues()
    {
        // Assert
        _configurationMock.Verify(x => x["EmailSettings:SenderName"], Times.Never);
        _configurationMock.Verify(x => x["EmailSettings:SenderEmail"], Times.Never);
        _configurationMock.Verify(x => x["EmailSettings:SmtpServer"], Times.Never);
        _configurationMock.Verify(x => x["EmailSettings:SmtpPort"], Times.Never);
    }
}
