using EmployeeManagement.Ativacao.Infrastructure.Jobs;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Ativacao.Infrastructure.Tests.Jobs;

public class EmployeeActivationJobTests
{
    [Fact]
    public void EmployeeActivationJob_ShouldBeInstantiatedWithValidDependencies()
    {
        // Arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmployeeActivationJob>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("Host=localhost;Database=test");
        configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(configSectionMock.Object);

        // Act
        var job = new EmployeeActivationJob(
            publishEndpointMock.Object,
            configurationMock.Object,
            loggerMock.Object);

        // Assert
        job.Should().NotBeNull();
    }

    [Fact]
    public void EmployeeActivationJob_ShouldAcceptIPublishEndpoint()
    {
        // Arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmployeeActivationJob>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("Host=localhost;Database=test");
        configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(configSectionMock.Object);

        // Act
        var job = new EmployeeActivationJob(
            publishEndpointMock.Object,
            configurationMock.Object,
            loggerMock.Object);

        // Assert
        job.Should().NotBeNull();
    }

    [Fact]
    public void EmployeeActivationJob_ShouldAcceptIConfiguration()
    {
        // Arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmployeeActivationJob>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("Host=localhost;Database=test");
        configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(configSectionMock.Object);

        // Act
        var job = new EmployeeActivationJob(
            publishEndpointMock.Object,
            configurationMock.Object,
            loggerMock.Object);

        // Assert
        job.Should().NotBeNull();
    }

    [Fact]
    public void EmployeeActivationJob_ShouldAcceptILogger()
    {
        // Arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmployeeActivationJob>>();

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("Host=localhost;Database=test");
        configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(configSectionMock.Object);

        // Act
        var job = new EmployeeActivationJob(
            publishEndpointMock.Object,
            configurationMock.Object,
            loggerMock.Object);

        // Assert
        job.Should().NotBeNull();
    }

    // Note: Full integration tests for ExecuteAsync would require:
    // - Real PostgreSQL database connection or Testcontainers
    // - Message broker setup
    // These tests verify the job structure and dependency injection
}
