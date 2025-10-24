using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Cadastro.Infrastructure.Messaging;
using FluentAssertions;
using MassTransit;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Messaging;

public class EventPublisherTests
{
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly EventPublisher _eventPublisher;

    public EventPublisherTests()
    {
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _eventPublisher = new EventPublisher(_publishEndpointMock.Object);
    }

    [Fact]
    public async Task PublishAsync_ValidEvent_ShouldCallPublishEndpoint()
    {
        // Arrange
        var @event = new EmployeeCreatedEvent
        {
            EmployeeId = Guid.NewGuid(),
            Name = "João Silva",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7),
            EventTime = DateTime.UtcNow
        };

        _publishEndpointMock
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishAsync(@event);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_EmployeeActivatedEvent_ShouldPublishCorrectly()
    {
        // Arrange
        var @event = new EmployeeActivatedEvent
        {
            EmployeeId = Guid.NewGuid(),
            EmployeeName = "Maria Santos",
            Department = "RH",
            ActivatedAt = DateTime.UtcNow
        };

        _publishEndpointMock
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishAsync(@event);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_EmployeeStartDateUpdatedEvent_ShouldPublishCorrectly()
    {
        // Arrange
        var @event = new EmployeeStartDateUpdatedEvent
        {
            EmployeeId = Guid.NewGuid(),
            EmployeeName = "Pedro Oliveira",
            Department = "Vendas",
            OldStartDate = DateTime.UtcNow.AddDays(7),
            NewStartDate = DateTime.UtcNow.AddDays(14),
            EventTime = DateTime.UtcNow
        };

        _publishEndpointMock
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishAsync(@event);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_MultipleEvents_ShouldPublishAll()
    {
        // Arrange
        var event1 = new EmployeeCreatedEvent
        {
            EmployeeId = Guid.NewGuid(),
            Name = "Emp 1",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7),
            EventTime = DateTime.UtcNow
        };

        var event2 = new EmployeeCreatedEvent
        {
            EmployeeId = Guid.NewGuid(),
            Name = "Emp 2",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(10),
            EventTime = DateTime.UtcNow
        };

        _publishEndpointMock
            .Setup(x => x.Publish(It.IsAny<EmployeeCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishAsync(event1);
        await _eventPublisher.PublishAsync(event2);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(It.IsAny<EmployeeCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task PublishAsync_GenericEvent_ShouldAcceptAnyClassType()
    {
        // Arrange
        var customEvent = new CustomTestEvent { Message = "Test" };

        _publishEndpointMock
            .Setup(x => x.Publish(customEvent, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishAsync(customEvent);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(customEvent, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Helper class for testing generic event publishing
    private class CustomTestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
}
