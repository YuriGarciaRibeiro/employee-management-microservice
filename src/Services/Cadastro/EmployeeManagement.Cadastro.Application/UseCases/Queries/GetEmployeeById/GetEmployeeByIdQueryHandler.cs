namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(request.Id);
        return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
    }
}