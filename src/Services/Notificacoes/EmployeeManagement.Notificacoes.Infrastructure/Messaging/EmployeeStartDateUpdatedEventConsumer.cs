using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeStartDateUpdatedEventConsumer : IConsumer<EmployeeStartDateUpdatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<EmployeeStartDateUpdatedEventConsumer> _logger;

    public EmployeeStartDateUpdatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeStartDateUpdatedEventConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeStartDateUpdatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Recebido evento EmployeeStartDateUpdatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Nova Data: {NewStartDate}",
            @event.EmployeeId, @event.EmployeeName, @event.NewStartDate);

        try
        {
            await _notificationService.NotifyStartDateUpdatedAsync(
                @event.EmployeeId,
                @event.EmployeeName,
                @event.Department,
                @event.NewStartDate);

            _logger.LogInformation(
                "Notificação de atualização de data enviada com sucesso para o funcionário {EmployeeId}",
                @event.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao processar notificação de atualização de data do funcionário {EmployeeId}",
                @event.EmployeeId);
            throw;
        }
    }
}
