using EmployeeManagement.Notificacoes.Application.Services;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using EmployeeManagement.Notificacoes.Infrastructure.Messaging;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Notificacoes.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando Notificacoes.API");

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Employee Management - Notificações API",
            Version = "v1",
            Description = "API de notificações em tempo real usando SignalR. O SignalR Hub está disponível em /employeeHub"
        });
    });

    // Configurar CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Configurar SignalR
    builder.Services.AddSignalR();

    // Registrar serviços da aplicação
    builder.Services.AddScoped<INotificationService, NotificationService>();

    // Configurar MassTransit (RabbitMQ)
    var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
    var host = rabbitMqConfig["Host"] ?? "localhost";
    var port = rabbitMqConfig.GetValue<ushort>("Port", 5672);
    var username = rabbitMqConfig["Username"] ?? "guest";
    var password = rabbitMqConfig["Password"] ?? "guest";

    builder.Services.AddMassTransit(x =>
    {
        // Registrar consumers
        x.AddConsumer<EmployeeCreatedEventConsumer>();
        x.AddConsumer<EmployeeActivatedEventConsumer>();
        x.AddConsumer<EmployeeStartDateUpdatedEventConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(host, port, "/", h =>
            {
                h.Username(username);
                h.Password(password);
            });

            // Configurar endpoints para os consumers
            cfg.ReceiveEndpoint("employee-notifications-created", e =>
            {
                e.ConfigureConsumer<EmployeeCreatedEventConsumer>(context);
            });

            cfg.ReceiveEndpoint("employee-notifications-activated", e =>
            {
                e.ConfigureConsumer<EmployeeActivatedEventConsumer>(context);
            });

            cfg.ReceiveEndpoint("employee-notifications-startdate-updated", e =>
            {
                e.ConfigureConsumer<EmployeeStartDateUpdatedEventConsumer>(context);
            });
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseCors("AllowAll");

    // Habilitar arquivos estáticos (HTML, CSS, JS)
    app.UseStaticFiles();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // Mapear SignalR Hub
    app.MapHub<EmployeeHub>("/employeeHub");

    Log.Information("SignalR Hub mapeado em: /employeeHub");
    Log.Information("Notificacoes.API iniciado com sucesso");

    app.Run();

    Log.Information("Notificacoes.API encerrado com sucesso");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Notificacoes.API terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
