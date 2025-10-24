using System.Net;
using System.Text.Json;
using EmployeeManagement.Cadastro.API.Middleware;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.API.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WithNoException_ShouldCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithKeyNotFoundException_ShouldReturn404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var errorMessage = "Funcionário não encontrado";
        var exception = new KeyNotFoundException(errorMessage);

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response.Should().NotBeNull();
        response!.status.Should().Be(404);
        response.message.Should().Be(errorMessage);
        response.timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task InvokeAsync_WithValidationException_ShouldReturn422()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Nome é obrigatório"),
            new ValidationFailure("Email", "Email inválido")
        };
        var validationException = new ValidationException(validationFailures);

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response.Should().NotBeNull();
        response!.status.Should().Be(422);
        response.message.Should().Contain("Nome é obrigatório");
        response.message.Should().Contain("Email inválido");
    }

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedAccessException_ShouldReturn401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new UnauthorizedAccessException();

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response.Should().NotBeNull();
        response!.status.Should().Be(401);
        response.message.Should().Be("Não autorizado");
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_ShouldReturn500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Something went wrong");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response.Should().NotBeNull();
        response!.status.Should().Be(500);
        response.message.Should().Be("Ocorreu um erro interno no servidor");
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithMultipleValidationErrors_ShouldCombineMessages()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Field1", "Erro 1"),
            new ValidationFailure("Field2", "Erro 2"),
            new ValidationFailure("Field3", "Erro 3")
        };
        var validationException = new ValidationException(validationFailures);

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response!.message.Should().Contain("Erro 1");
        response.message.Should().Contain("Erro 2");
        response.message.Should().Contain("Erro 3");
        response.message.Should().Contain(";");
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetCorrectContentType()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_ResponseTimestamp_ShouldBeUtcNow()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test");
        var beforeTest = DateTime.UtcNow;

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);
        var afterTest = DateTime.UtcNow;

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        response!.timestamp.Should().BeOnOrAfter(beforeTest);
        response.timestamp.Should().BeOnOrBefore(afterTest);
    }

    // Helper class for deserializing response
    private class ErrorResponse
    {
        public int status { get; set; }
        public string message { get; set; } = string.Empty;
        public DateTime timestamp { get; set; }
    }
}
