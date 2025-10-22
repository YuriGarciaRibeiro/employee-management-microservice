namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeDto Employee) : IRequest<EmployeeDto>;