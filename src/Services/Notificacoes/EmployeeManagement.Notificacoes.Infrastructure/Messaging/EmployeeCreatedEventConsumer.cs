using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using EmployeeManagement.Notificacoes.Infrastructure.Messaging.Base;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeCreatedEventConsumer : BaseEventConsumer<EmployeeCreatedEvent>
{
    private readonly INotificationService _notificationService;

    public EmployeeCreatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeCreatedEventConsumer> logger)
        : base(logger)
    {
        _notificationService = notificationService;
    }

    protected override async Task ProcessEventAsync(EmployeeCreatedEvent @event)
    {
        await _notificationService.NotifyEmployeeCreatedAsync(
            @event.EmployeeId,
            @event.Name,
            @event.Department,
            @event.StartDate);
    }

    protected override void LogEventReceived(EmployeeCreatedEvent @event)
    {
        Logger.LogInformation(
            "Recebido evento EmployeeCreatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Departamento: {Department}",
            @event.EmployeeId, @event.Name, @event.Department);
    }

    protected override void LogEventProcessedSuccessfully(EmployeeCreatedEvent @event)
    {
        Logger.LogInformation(
            "Notificação de criação enviada com sucesso para o funcionário {EmployeeId}",
            @event.EmployeeId);
    }

    protected override void LogEventProcessingError(EmployeeCreatedEvent @event, Exception ex)
    {
        Logger.LogError(ex,
            "Erro ao processar notificação de criação do funcionário {EmployeeId}",
            @event.EmployeeId);
    }
}
