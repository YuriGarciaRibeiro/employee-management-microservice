using EmployeeManagement.Ativacao.Infrastructure;
using EmployeeManagement.Ativacao.Infrastructure.Jobs;
using EmployeeManagement.BuildingBlocks.Core.Extensions;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

// Configurar Serilog
builder.AddSerilogLogging("Ativacao.Worker");

// Infrastructure (Hangfire, DbContext, Repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();

// Configurar Hangfire Job recorrente
using (var scope = host.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    recurringJobManager.AddOrUpdate<EmployeeActivationJob>(
        "employee-activation-job",
        job => job.ExecuteAsync(),
        Cron.Minutely(),
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Local
        });

    logger.LogInformation("Job de ativação de funcionários configurado com sucesso");
}

// Executar worker com logging do Serilog
SerilogExtensions.RunWithSerilog(() => host.Run(), "Ativacao.Worker");
