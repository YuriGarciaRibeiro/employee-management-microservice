namespace EmployeeManagement.Cadastro.Application.Validators;

public class UpdateStartDateDtoValidator : AbstractValidator<UpdateStartDateDto>
{
    public UpdateStartDateDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Nova data de início é obrigatória")
            .Must(date => date.Date >= DateTime.UtcNow.Date).WithMessage("Nova data de início não pode ser no passado");
    }
}
