using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesByDateRange;

public class GetEmployeesByDateRangeQueryValidator : AbstractValidator<GetEmployeesByDateRangeQuery>
{
    public GetEmployeesByDateRangeQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data inicial é obrigatória")
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Data inicial deve ser anterior ou igual à data final");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Data final é obrigatória");
    }
}
