namespace EmployeeManagement.BuildingBlocks.Contracts.Events;

public class ActivateEmployeeBatchEvent
{
    public List<EmployeeBatchDto> Employees { get; set; } = new();
    public DateTime ScheduledDate { get; set; }
    public int BatchNumber { get; set; }
    public int TotalBatches { get; set; }
}

public class EmployeeBatchDto
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}
