namespace EmployeeManagement.Notificacoes.Application.DTOs;

public record EmployeeNotificationDto
{
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
