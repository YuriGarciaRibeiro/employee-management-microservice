using FluentValidation;

namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;

public class UpdateStartDateCommandValidator : AbstractValidator<UpdateStartDateCommand>
{
    public UpdateStartDateCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("ID do funcionário é obrigatório");

        RuleFor(x => x.NewStartDate)
            .NotEmpty().WithMessage("Data de início é obrigatória")
            .Must(date => date.Date >= DateTime.UtcNow.Date).WithMessage("Data de início não pode ser no passado");
    }
}
