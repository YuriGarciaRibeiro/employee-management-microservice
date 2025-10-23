using EmployeeManagement.BuildingBlocks.Contracts.Events;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging;

public class EmployeeActivatedEventConsumer : IConsumer<EmployeeActivatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<EmployeeActivatedEventConsumer> _logger;

    public EmployeeActivatedEventConsumer(
        INotificationService notificationService,
        ILogger<EmployeeActivatedEventConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeActivatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Recebido evento EmployeeActivatedEvent. EmployeeId: {EmployeeId}, Nome: {Name}, Departamento: {Department}",
            @event.EmployeeId, @event.EmployeeName, @event.Department);

        try
        {
            await _notificationService.NotifyEmployeeActivatedAsync(
                @event.EmployeeId,
                @event.EmployeeName,
                @event.Department);

            _logger.LogInformation(
                "Notificação de ativação enviada com sucesso para o funcionário {EmployeeId}",
                @event.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao processar notificação de ativação do funcionário {EmployeeId}",
                @event.EmployeeId);
            throw;
        }
    }
}
