using AutoMapper;
using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Handlers.CreateEmployee;

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<CreateEmployeeCommandHandler>> _loggerMock;
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<CreateEmployeeCommandHandler>>();
        _handler = new CreateEmployeeCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _emailServiceMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidEmployee_ShouldCreateEmployee()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            Name = "João Silva",
            Phone = "+5511999999999",
            Department = "TI",
            StartDate = DateTime.UtcNow.AddDays(7)
        };
        var command = new CreateEmployeeCommand(dto);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Phone = dto.Phone,
            Department = dto.Department,
            StartDate = dto.StartDate,
            Status = EmployeeStatus.Inativo
        };

        _mapperMock.Setup(x => x.Map<Employee>(dto)).Returns(employee);
        _mapperMock.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Phone = employee.Phone,
            Department = employee.Department,
            StartDate = employee.StartDate,
            Status = EmployeeStatus.Inativo,
        });

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Employee>())).ReturnsAsync(employee);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Phone.Should().Be(dto.Phone);
        result.Department.Should().Be(dto.Department);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidEmployee_ShouldPublishEvent()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            Name = "Maria Santos",
            Phone = "+5511988888888",
            Department = "RH",
            StartDate = DateTime.UtcNow.AddDays(10)
        };
        var command = new CreateEmployeeCommand(dto);

        var employee = new Employee { Id = Guid.NewGuid(), Name = dto.Name, Phone = dto.Phone, Department = "RH", StartDate = dto.StartDate };
        _mapperMock.Setup(x => x.Map<Employee>(dto)).Returns(employee);
        _mapperMock.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(new EmployeeDto());
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Employee>())).ReturnsAsync(employee);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
    }
}
