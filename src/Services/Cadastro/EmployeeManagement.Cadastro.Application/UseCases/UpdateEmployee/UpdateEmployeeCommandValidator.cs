using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Employee.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Employee.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MinimumLength(10).WithMessage("Telefone deve ter no mínimo 10 caracteres")
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .Matches(@"^\+?[1-9]\d{9,18}$").WithMessage("Telefone inválido. Use formato: +5511999999999 ou 11999999999");

        RuleFor(x => x.Employee.Department)
            .NotEmpty().WithMessage("Setor é obrigatório")
            .MaximumLength(50).WithMessage("Setor deve ter no máximo 50 caracteres");
    }
}
