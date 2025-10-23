namespace EmployeeManagement.BuildingBlocks.Contracts.Events;

/// <summary>
/// Evento publicado quando um lote de ativações é concluído.
/// </summary>
public class BatchCompletedEvent
{
    public Guid BatchId { get; set; }
    public int TotalEmployees { get; set; }
    public int ProcessedEmployees { get; set; }
    public DateTime CompletedAt { get; set; }
}
