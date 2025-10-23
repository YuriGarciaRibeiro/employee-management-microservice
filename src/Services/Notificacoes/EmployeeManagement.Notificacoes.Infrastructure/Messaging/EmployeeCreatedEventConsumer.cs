using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;

    public EmployeeCreatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeCreatedEventConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Recebido evento EmployeeCreatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Departamento: {Department}",
            @event.EmployeeId, @event.Name, @event.Department);

        try
        {
            await _notificationService.NotifyEmployeeCreatedAsync(
                @event.EmployeeId,
                @event.Name,
                @event.Department,
                @event.StartDate);

            _logger.LogInformation(
                "Notificação de criação enviada com sucesso para o funcionário {EmployeeId}",
                @event.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao processar notificação de criação do funcionário {EmployeeId}",
                @event.EmployeeId);
            throw;
        }
    }
}
