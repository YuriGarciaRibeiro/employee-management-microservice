using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployees;

public class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Página deve ser maior que 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Tamanho da página deve ser maior que 0")
            .LessThanOrEqualTo(100).WithMessage("Tamanho da página não pode exceder 100");
    }
}
