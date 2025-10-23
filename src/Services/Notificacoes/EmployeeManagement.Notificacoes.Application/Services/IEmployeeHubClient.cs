using EmployeeManagement.Notificacoes.Application.DTOs;

namespace EmployeeManagement.Notificacoes.Application.Services;

public interface IEmployeeHubClient
{
    Task ReceiveNotification(EmployeeNotificationDto notification);
    Task ReceiveEmployeeCreated(EmployeeNotificationDto notification);
    Task ReceiveEmployeeActivated(EmployeeNotificationDto notification);
}
