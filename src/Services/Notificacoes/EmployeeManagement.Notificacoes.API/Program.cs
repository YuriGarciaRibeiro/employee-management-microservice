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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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
