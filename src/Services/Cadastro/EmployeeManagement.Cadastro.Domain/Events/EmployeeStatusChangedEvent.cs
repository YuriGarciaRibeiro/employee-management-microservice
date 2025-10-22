using EmployeeManagement.Cadastro.Domain.Enums;

namespace EmployeeManagement.Cadastro.Domain.Events;

public class EmployeeStatusChangedEvent
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public EmployeeStatus OldStatus { get; set; }
    public EmployeeStatus NewStatus { get; set; }
    public DateTime EventTime { get; set; }
}
