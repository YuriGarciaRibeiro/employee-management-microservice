using EmployeeManagement.Notificacoes.Infrastructure.Messaging.Base;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Notificacoes.Infrastructure.Tests.Messaging;

public class BaseEventConsumerTests
{
    private readonly Mock<ILogger<TestEventConsumer>> _loggerMock;
    private readonly Mock<ConsumeContext<TestEvent>> _contextMock;

    public BaseEventConsumerTests()
    {
        _loggerMock = new Mock<ILogger<TestEventConsumer>>();
        _contextMock = new Mock<ConsumeContext<TestEvent>>();
    }

    [Fact]
    public async Task Consume_WithValidEvent_ShouldProcessSuccessfully()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), Message = "Test Message" };
        _contextMock.Setup(x => x.Message).Returns(testEvent);

        var consumer = new TestEventConsumer(_loggerMock.Object);

        // Act
        await consumer.Consume(_contextMock.Object);

        // Assert
        consumer.ProcessEventCalled.Should().BeTrue();
        consumer.LogEventReceivedCalled.Should().BeTrue();
        consumer.LogEventProcessedSuccessfullyCalled.Should().BeTrue();
        consumer.LogEventProcessingErrorCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Consume_WhenProcessingThrowsException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), Message = "Test Message" };
        _contextMock.Setup(x => x.Message).Returns(testEvent);

        var expectedError = new InvalidOperationException("Processing failed");
        var consumer = new TestEventConsumer(_loggerMock.Object, shouldThrow: true, exceptionToThrow: expectedError);

        // Act
        Func<Task> act = async () => await consumer.Consume(_contextMock.Object);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Processing failed");

        consumer.ProcessEventCalled.Should().BeTrue();
        consumer.LogEventReceivedCalled.Should().BeTrue();
        consumer.LogEventProcessedSuccessfullyCalled.Should().BeFalse();
        consumer.LogEventProcessingErrorCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Consume_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), Message = "Test Message" };
        _contextMock.Setup(x => x.Message).Returns(testEvent);

        var consumer = new TestEventConsumer(_loggerMock.Object);
        var callOrder = new List<string>();
        consumer.OnLogEventReceived = () => callOrder.Add("LogReceived");
        consumer.OnProcessEvent = () => callOrder.Add("Process");
        consumer.OnLogEventProcessedSuccessfully = () => callOrder.Add("LogSuccess");

        // Act
        await consumer.Consume(_contextMock.Object);

        // Assert
        callOrder.Should().Equal("LogReceived", "Process", "LogSuccess");
    }

    [Fact]
    public async Task Consume_WhenExceptionOccurs_ShouldNotCallSuccessLog()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), Message = "Test Message" };
        _contextMock.Setup(x => x.Message).Returns(testEvent);

        var consumer = new TestEventConsumer(_loggerMock.Object, shouldThrow: true);

        // Act
        try
        {
            await consumer.Consume(_contextMock.Object);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        consumer.LogEventProcessedSuccessfullyCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Consume_WithDifferentEventTypes_ShouldProcessCorrectly()
    {
        // Arrange
        var testEvent1 = new TestEvent { Id = Guid.NewGuid(), Message = "Event 1" };
        var testEvent2 = new TestEvent { Id = Guid.NewGuid(), Message = "Event 2" };

        var context1 = new Mock<ConsumeContext<TestEvent>>();
        context1.Setup(x => x.Message).Returns(testEvent1);

        var context2 = new Mock<ConsumeContext<TestEvent>>();
        context2.Setup(x => x.Message).Returns(testEvent2);

        var consumer = new TestEventConsumer(_loggerMock.Object);

        // Act
        await consumer.Consume(context1.Object);
        await consumer.Consume(context2.Object);

        // Assert
        consumer.ProcessedEvents.Should().HaveCount(2);
        consumer.ProcessedEvents.Should().Contain(testEvent1);
        consumer.ProcessedEvents.Should().Contain(testEvent2);
    }

    // Test event class
    public class TestEvent
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // Test consumer implementation
    public class TestEventConsumer : BaseEventConsumer<TestEvent>
    {
        private readonly bool _shouldThrow;
        private readonly Exception? _exceptionToThrow;

        public bool ProcessEventCalled { get; private set; }
        public bool LogEventReceivedCalled { get; private set; }
        public bool LogEventProcessedSuccessfullyCalled { get; private set; }
        public bool LogEventProcessingErrorCalled { get; private set; }
        public List<TestEvent> ProcessedEvents { get; } = new();

        public Action? OnLogEventReceived { get; set; }
        public Action? OnProcessEvent { get; set; }
        public Action? OnLogEventProcessedSuccessfully { get; set; }

        public TestEventConsumer(ILogger logger, bool shouldThrow = false, Exception? exceptionToThrow = null)
            : base(logger)
        {
            _shouldThrow = shouldThrow;
            _exceptionToThrow = exceptionToThrow;
        }

        protected override async Task ProcessEventAsync(TestEvent @event)
        {
            ProcessEventCalled = true;
            OnProcessEvent?.Invoke();
            ProcessedEvents.Add(@event);

            if (_shouldThrow)
            {
                throw _exceptionToThrow ?? new InvalidOperationException("Test exception");
            }

            await Task.CompletedTask;
        }

        protected override void LogEventReceived(TestEvent @event)
        {
            LogEventReceivedCalled = true;
            OnLogEventReceived?.Invoke();
        }

        protected override void LogEventProcessedSuccessfully(TestEvent @event)
        {
            LogEventProcessedSuccessfullyCalled = true;
            OnLogEventProcessedSuccessfully?.Invoke();
        }

        protected override void LogEventProcessingError(TestEvent @event, Exception ex)
        {
            LogEventProcessingErrorCalled = true;
        }
    }
}
