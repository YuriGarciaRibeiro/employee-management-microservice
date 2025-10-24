namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesGroupedByDepartment;

public class GetEmployeesGroupedByDepartmentQueryHandler : IRequestHandler<GetEmployeesGroupedByDepartmentQuery, List<EmployeeGroupedByDepartmentDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeesGroupedByDepartmentQueryHandler(IEmployeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeGroupedByDepartmentDto>> Handle(GetEmployeesGroupedByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var grouped = await _repository.GetGroupedByDepartmentAsync(request.StartDate, request.EndDate);
    
        return grouped.Select(g => new EmployeeGroupedByDepartmentDto
        {
            Department = g.Key,
            EmployeeCount = g.Count(),
            Employees = _mapper.Map<List<EmployeeDto>>(g.ToList())
        }).ToList();
    }
}