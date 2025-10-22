namespace EmployeeManagement.Cadastro.Domain.Events;

public class EmployeeCreatedEvent
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EventTime { get; set; }
}
