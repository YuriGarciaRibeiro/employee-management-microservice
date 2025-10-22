namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _repository;

    public UpdateEmployeeCommandHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Funcionário com ID {request.Id} não encontrado");

        employee.Name = request.Employee.Name;
        employee.Phone = request.Employee.Phone;
        employee.Department = request.Employee.Department;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(employee);

        return Unit.Value;
    }
}