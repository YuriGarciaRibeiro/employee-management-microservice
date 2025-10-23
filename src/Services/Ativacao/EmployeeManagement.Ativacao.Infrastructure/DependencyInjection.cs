namespace EmployeeManagement.Ativacao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ActivateEmployeeBatchConsumer>();

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

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection")
                    ?? configuration.GetConnectionString("DefaultConnection")!);
            }, new Hangfire.PostgreSql.PostgreSqlStorageOptions
            {
                SchemaName = "hangfire"
            }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.ServerName = "AtivacaoWorker";
        });

        services.AddScoped<EmployeeActivationJob>();

        return services;
    }
}