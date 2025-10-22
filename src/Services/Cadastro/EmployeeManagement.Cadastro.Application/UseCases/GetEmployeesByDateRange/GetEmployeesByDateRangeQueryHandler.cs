namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesByDateRange;

public class GetEmployeesByDateRangeQueryHandler : IRequestHandler<GetEmployeesByDateRangeQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeesByDateRangeQueryHandler(IEmployeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>> Handle(GetEmployeesByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var employees = await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}