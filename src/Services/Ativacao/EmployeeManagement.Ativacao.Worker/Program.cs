using EmployeeManagement.Ativacao.Infrastructure;
using EmployeeManagement.Ativacao.Infrastructure.Jobs;
using Hangfire;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Ativacao.Worker")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Services.AddSerilog();

try
{
    Log.Information("Iniciando Ativacao.Worker");

builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();

// Configurar Hangfire Jobs
using (var scope = host.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<EmployeeActivationJob>(
        "employee-activation-job",
        job => job.ExecuteAsync(),
        Cron.Minutely(),
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Local
        });

    Log.Information("Job de ativação de funcionários configurado com sucesso");
}

    host.Run();

    Log.Information("Ativacao.Worker encerrado com sucesso");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Ativacao.Worker terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
