namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto?>;