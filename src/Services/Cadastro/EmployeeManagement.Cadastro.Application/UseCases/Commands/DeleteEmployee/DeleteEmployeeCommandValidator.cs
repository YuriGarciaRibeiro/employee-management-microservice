using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID do funcionário é obrigatório");
    }
}
