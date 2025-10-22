namespace EmployeeManagement.Cadastro.Domain.Events;

public class EmployeeStartDateUpdatedEvent
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime OldStartDate { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime EventTime { get; set; }
}
