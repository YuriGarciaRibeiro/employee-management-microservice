using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.BuildingBlocks.Messaging.Extensions;

/// <summary>
/// Extensões para configurar RabbitMQ com MassTransit de forma centralizada.
/// Centraliza a versão do MassTransit e configuração padrão.
/// </summary>
public static class RabbitMqExtensions
{
    /// <summary>
    /// Adiciona MassTransit com RabbitMQ usando configuração padrão.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="configure">Ação para configurar consumidores e endpoints</param>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(x =>
        {
            // Permite registrar consumers via callback
            configure?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
