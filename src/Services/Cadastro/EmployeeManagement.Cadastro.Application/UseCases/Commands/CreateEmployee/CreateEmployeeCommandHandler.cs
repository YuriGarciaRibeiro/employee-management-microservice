namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;

using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Cadastro.Application.Helpers;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository repository,
        IMapper mapper,
        IEmailService emailService,
        IEventPublisher eventPublisher,
        ILogger<CreateEmployeeCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _emailService = emailService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = _mapper.Map<Employee>(request.Employee);
        employee.Id = Guid.NewGuid();

        var createdEmployee = await _repository.AddAsync(employee);

        _ = Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation(
                    "Tentando enviar email de boas-vindas para {EmployeeName} ({Email})",
                    employee.Name, employee.Email);

                await _emailService.SendWelcomeEmailAsync(
                    employee.Email,
                    employee.Name,
                    employee.StartDate
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao enviar email de boas-vindas para {EmployeeName} ({Email}). " +
                    "Verifique as configurações SMTP no arquivo .env",
                    employee.Name, employee.Email);
            }
        }, cancellationToken);

        var @event = new EmployeeCreatedEvent
        {
            EmployeeId = createdEmployee.Id,
            Name = createdEmployee.Name,
            Department = createdEmployee.Department,
            StartDate = createdEmployee.StartDate,
            EventTime = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(@event);

        return _mapper.Map<EmployeeDto>(createdEmployee);
    }
}
