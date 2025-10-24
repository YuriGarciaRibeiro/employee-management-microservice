using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using EmployeeManagement.Notificacoes.Infrastructure.Messaging.Base;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeActivatedEventConsumer : BaseEventConsumer<EmployeeActivatedEvent>
{
    private readonly INotificationService _notificationService;

    public EmployeeActivatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeActivatedEventConsumer> logger)
        : base(logger)
    {
        _notificationService = notificationService;
    }

    protected override async Task ProcessEventAsync(EmployeeActivatedEvent @event)
    {
        await _notificationService.NotifyEmployeeActivatedAsync(
            @event.EmployeeId,
            @event.EmployeeName,
            @event.Department);
    }

    protected override void LogEventReceived(EmployeeActivatedEvent @event)
    {
        Logger.LogInformation(
            "Recebido evento EmployeeActivatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Departamento: {Department}",
            @event.EmployeeId, @event.EmployeeName, @event.Department);
    }

    protected override void LogEventProcessedSuccessfully(EmployeeActivatedEvent @event)
    {
        Logger.LogInformation(
            "Notificação de ativação enviada com sucesso para o funcionário {EmployeeId}",
            @event.EmployeeId);
    }

    protected override void LogEventProcessingError(EmployeeActivatedEvent @event, Exception ex)
    {
        Logger.LogError(ex,
            "Erro ao processar notificação de ativação do funcionário {EmployeeId}",
            @event.EmployeeId);
    }
}
