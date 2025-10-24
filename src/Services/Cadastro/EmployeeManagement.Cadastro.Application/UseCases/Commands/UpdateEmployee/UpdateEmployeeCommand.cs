namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeDto Employee) : IRequest<Unit>;