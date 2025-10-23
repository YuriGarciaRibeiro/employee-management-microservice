namespace EmployeeManagement.BuildingBlocks.Contracts.Events;

/// <summary>
/// Evento publicado quando um novo funcionário é criado no sistema.
/// Este evento é compartilhado entre os microsserviços de Cadastro e Ativação.
/// </summary>
public class EmployeeCreatedEvent
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EventTime { get; set; }
}
