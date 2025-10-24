using EmployeeManagement.Notificacoes.Application.Services;
using EmployeeManagement.Notificacoes.Domain.Interfaces;
using EmployeeManagement.Notificacoes.Infrastructure.Messaging;
using EmployeeManagement.BuildingBlocks.Core.Extensions;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.AddSerilogLogging("Notificacoes.API");

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerDocumentation(
    title: "Employee Management - Notificações API",
    description: "API de notificações em tempo real usando SignalR. O SignalR Hub está disponível em /employeeHub");

// CORS
builder.Services.AddDevelopmentCors();

// SignalR
builder.Services.AddSignalR();

// Serviços da aplicação
builder.Services.AddScoped<INotificationService, NotificationService>();

// MassTransit (RabbitMQ) com consumers
builder.Services.AddMassTransit(x =>
{
    // Registrar consumers
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeActivatedEventConsumer>();
    x.AddConsumer<EmployeeStartDateUpdatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbitMqConfig["Host"] ?? "localhost",
                 rabbitMqConfig.GetValue<ushort>("Port", 5672),
                 "/",
                 h =>
                 {
                     h.Username(rabbitMqConfig["Username"] ?? "guest");
                     h.Password(rabbitMqConfig["Password"] ?? "guest");
                 });

        // Configurar endpoints
        cfg.ReceiveEndpoint("employee-notifications-created", e =>
            e.ConfigureConsumer<EmployeeCreatedEventConsumer>(context));

        cfg.ReceiveEndpoint("employee-notifications-activated", e =>
            e.ConfigureConsumer<EmployeeActivatedEventConsumer>(context));

        cfg.ReceiveEndpoint("employee-notifications-startdate-updated", e =>
            e.ConfigureConsumer<EmployeeStartDateUpdatedEventConsumer>(context));
    });
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCorsPolicy");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<EmployeeHub>("/employeeHub");

// Executar aplicação com logging do Serilog
SerilogExtensions.RunWithSerilog(() => app.Run(), "Notificacoes.API");
