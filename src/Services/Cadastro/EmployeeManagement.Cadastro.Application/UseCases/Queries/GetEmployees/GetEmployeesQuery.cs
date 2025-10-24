using EmployeeManagement.Cadastro.Application.DTOs;
using MediatR;

namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployees;

public record GetEmployeesQuery(int Page, int PageSize) : IRequest<PaginatedResultDto<EmployeeDto>>;