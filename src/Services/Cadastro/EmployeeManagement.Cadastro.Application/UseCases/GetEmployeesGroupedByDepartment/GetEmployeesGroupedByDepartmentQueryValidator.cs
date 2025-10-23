using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesGroupedByDepartment;

public class GetEmployeesGroupedByDepartmentQueryValidator : AbstractValidator<GetEmployeesGroupedByDepartmentQuery>
{
    public GetEmployeesGroupedByDepartmentQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate.Value <= x.EndDate.Value)
            .WithMessage("Data inicial deve ser anterior ou igual à data final");
    }
}
