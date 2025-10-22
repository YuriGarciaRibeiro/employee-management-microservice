namespace EmployeeManagement.Cadastro.Application.Validators;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve conter 10 ou 11 dígitos");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data de início é obrigatória")
            .Must(date => date.Date >= DateTime.UtcNow.Date).WithMessage("Data de início não pode ser no passado");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Setor é obrigatório")
            .MaximumLength(50).WithMessage("Setor deve ter no máximo 50 caracteres");
    }
}