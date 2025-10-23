namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;

using EmployeeManagement.BuildingBlocks.Contracts.Events;

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
                await _emailService.SendWelcomeEmailAsync(
                    $"{employee.Name.ToLower().Replace(" ", ".")}@empresa.com",
                    employee.Name,
                    employee.StartDate
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email");
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
