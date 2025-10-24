namespace EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;

public record UpdateStartDateCommand(Guid EmployeeId, DateTime NewStartDate) : IRequest<Unit>;