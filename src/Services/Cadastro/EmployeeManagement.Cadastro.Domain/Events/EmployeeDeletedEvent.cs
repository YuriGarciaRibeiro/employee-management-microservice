namespace EmployeeManagement.Cadastro.Domain.Events;

public class EmployeeDeletedEvent
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}
