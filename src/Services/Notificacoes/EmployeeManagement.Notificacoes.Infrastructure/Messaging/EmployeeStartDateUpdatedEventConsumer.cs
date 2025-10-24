using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using EmployeeManagement.Notificacoes.Infrastructure.Messaging.Base;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeStartDateUpdatedEventConsumer : BaseEventConsumer<EmployeeStartDateUpdatedEvent>
{
    private readonly INotificationService _notificationService;

    public EmployeeStartDateUpdatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeStartDateUpdatedEventConsumer> logger)
        : base(logger)
    {
        _notificationService = notificationService;
    }

    protected override async Task ProcessEventAsync(EmployeeStartDateUpdatedEvent @event)
    {
        await _notificationService.NotifyStartDateUpdatedAsync(
            @event.EmployeeId,
            @event.EmployeeName,
            @event.Department,
            @event.NewStartDate);
    }

    protected override void LogEventReceived(EmployeeStartDateUpdatedEvent @event)
    {
        Logger.LogInformation(
            "Recebido evento EmployeeStartDateUpdatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Nova Data: {NewStartDate}",
            @event.EmployeeId, @event.EmployeeName, @event.NewStartDate);
    }

    protected override void LogEventProcessedSuccessfully(EmployeeStartDateUpdatedEvent @event)
    {
        Logger.LogInformation(
            "Notificação de atualização de data enviada com sucesso para o funcionário {EmployeeId}",
            @event.EmployeeId);
    }

    protected override void LogEventProcessingError(EmployeeStartDateUpdatedEvent @event, Exception ex)
    {
        Logger.LogError(ex,
            "Erro ao processar notificação de atualização de data do funcionário {EmployeeId}",
            @event.EmployeeId);
    }
}
