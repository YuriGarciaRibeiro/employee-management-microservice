using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Cadastro.Application.Helpers;

namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;

public class UpdateStartDateCommandHandler : IRequestHandler<UpdateStartDateCommand, Unit>
{
    private readonly IEmployeeRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdateStartDateCommandHandler> _logger;

    public UpdateStartDateCommandHandler(
        IEmployeeRepository repository,
        IEventPublisher eventPublisher,
        IEmailService emailService,
        ILogger<UpdateStartDateCommandHandler> logger)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateStartDateCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(request.EmployeeId)
            ?? throw new KeyNotFoundException($"Funcionário com ID {request.EmployeeId} não encontrado");

        var oldStartDate = employee.StartDate;
        employee.StartDate = request.NewStartDate;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(employee);

        var @event = new EmployeeStartDateUpdatedEvent
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            Department = employee.Department,
            OldStartDate = oldStartDate,
            NewStartDate = request.NewStartDate,
            EventTime = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(@event);

        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendStartDateUpdatedEmailAsync(
                    EmailHelper.GenerateCompanyEmail(employee.Name),
                    employee.Name,
                    oldStartDate,
                    request.NewStartDate
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de atualização de data de início");
            }
        }, cancellationToken);

        return Unit.Value;
    }
}