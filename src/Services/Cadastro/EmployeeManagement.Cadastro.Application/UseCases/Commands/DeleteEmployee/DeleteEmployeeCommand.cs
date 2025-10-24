namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest<Unit>;