using EmployeeManagement.Notificacoes.Application.DTOs;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EmployeeManagement.Notificacoes.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<EmployeeHub, IEmployeeHubClient> _hubContext;

    public NotificationService(IHubContext<EmployeeHub, IEmployeeHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyEmployeeCreatedAsync(Guid employeeId, string employeeName, string department, DateTime startDate)
    {
        var notification = new EmployeeNotificationDto
        {
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            Department = department,
            StartDate = startDate,
            EventType = "EmployeeCreated",
            Message = $"Novo funcionário cadastrado: {employeeName} - Setor: {department}",
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients.Group(department)
            .ReceiveEmployeeCreated(notification);
    }

    public async Task NotifyEmployeeActivatedAsync(Guid employeeId, string employeeName, string department)
    {
        var notification = new EmployeeNotificationDto
        {
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            Department = department,
            EventType = "EmployeeActivated",
            Message = $"Funcionário ativado: {employeeName} - Setor: {department}",
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients.Group(department)
            .ReceiveEmployeeActivated(notification);
    }

    public Task NotifyStartDateUpdatedAsync(Guid employeeId, string employeeName, string department, DateTime newStartDate)
    {
        var notification = new EmployeeNotificationDto
        {
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            Department = department,
            StartDate = newStartDate,
            EventType = "StartDateUpdated",
            Message = $"Data de início atualizada para {employeeName}: {newStartDate:dd/MM/yyyy}",
            Timestamp = DateTime.UtcNow
        };

        return _hubContext.Clients.Group(department)
            .ReceiveNotification(notification);
    }
}
