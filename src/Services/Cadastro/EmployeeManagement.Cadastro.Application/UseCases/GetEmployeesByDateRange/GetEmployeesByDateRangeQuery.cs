namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesByDateRange;

public record GetEmployeesByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<EmployeeDto>>;