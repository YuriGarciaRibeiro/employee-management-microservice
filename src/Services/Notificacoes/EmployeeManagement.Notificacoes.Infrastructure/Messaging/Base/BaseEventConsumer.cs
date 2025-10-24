using MassTransit;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Infrastructure.Messaging.Base;

/// <summary>
/// Classe base abstrata para consumidores de eventos com tratamento padronizado de erros e logging.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento a ser consumido</typeparam>
public abstract class BaseEventConsumer<TEvent> : IConsumer<TEvent>
    where TEvent : class
{
    protected readonly ILogger Logger;

    protected BaseEventConsumer(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var @event = context.Message;

        LogEventReceived(@event);

        try
        {
            await ProcessEventAsync(@event);
            LogEventProcessedSuccessfully(@event);
        }
        catch (Exception ex)
        {
            LogEventProcessingError(@event, ex);
            throw;
        }
    }

    /// <summary>
    /// Método abstrato que deve ser implementado pelas classes derivadas para processar o evento.
    /// </summary>
    protected abstract Task ProcessEventAsync(TEvent @event);

    /// <summary>
    /// Método virtual que pode ser sobrescrito para customizar o log de recebimento do evento.
    /// </summary>
    protected abstract void LogEventReceived(TEvent @event);

    /// <summary>
    /// Método virtual que pode ser sobrescrito para customizar o log de sucesso.
    /// </summary>
    protected abstract void LogEventProcessedSuccessfully(TEvent @event);

    /// <summary>
    /// Método virtual que pode ser sobrescrito para customizar o log de erro.
    /// </summary>
    protected abstract void LogEventProcessingError(TEvent @event, Exception ex);
}
