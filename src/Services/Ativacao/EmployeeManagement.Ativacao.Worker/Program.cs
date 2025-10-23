using EmployeeManagement.Ativacao.Infrastructure;
using EmployeeManagement.Ativacao.Infrastructure.Jobs;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

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
}

host.Run();
