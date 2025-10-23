namespace EmployeeManagement.BuildingBlocks.Contracts.Events;

/// <summary>
/// Evento publicado quando um funcionário é ativado no sistema.
/// </summary>
public class EmployeeActivatedEvent
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime ActivatedAt { get; set; }
}
