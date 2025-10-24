namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesGroupedByDepartment;

public record GetEmployeesGroupedByDepartmentQuery(DateTime? StartDate, DateTime? EndDate) : IRequest<List<EmployeeGroupedByDepartmentDto>>;