namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public DeleteEmployeeCommandHandler(IEmployeeRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Funcionário com ID {request.Id} não encontrado");

        await _repository.DeleteAsync(request.Id);

        var @event = new EmployeeDeletedEvent
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            Department = employee.Department,
            DeletedAt = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(@event);

        return Unit.Value;
    }
}