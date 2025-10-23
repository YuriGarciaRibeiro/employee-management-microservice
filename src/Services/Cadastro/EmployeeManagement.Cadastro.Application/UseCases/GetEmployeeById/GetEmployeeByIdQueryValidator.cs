using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID do funcionário é obrigatório");
    }
}
