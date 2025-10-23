namespace EmployeeManagement.Notificacoes.Domain.Interfaces;

public interface INotificationService
{
    Task NotifyEmployeeCreatedAsync(Guid employeeId, string employeeName, string department, DateTime startDate);
    Task NotifyEmployeeActivatedAsync(Guid employeeId, string employeeName, string department);
    Task NotifyStartDateUpdatedAsync(Guid employeeId, string employeeName, string department, DateTime newStartDate);
}
