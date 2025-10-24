using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EmployeeManagement.BuildingBlocks.Core.Extensions;

/// <summary>
/// Extensões para configurar Serilog de forma centralizada.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Configura o Serilog para a aplicação com configurações padrão.
    /// </summary>
    /// <param name="builder">WebApplicationBuilder ou HostApplicationBuilder</param>
    /// <param name="applicationName">Nome da aplicação para enriquecimento de logs</param>
    public static void AddSerilogLogging(
        this IHostApplicationBuilder builder,
        string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .CreateLogger();

        builder.Services.AddSerilog();
    }

    /// <summary>
    /// Executa uma aplicação com tratamento de exceções e logging do Serilog.
    /// </summary>
    /// <param name="action">Ação a ser executada (geralmente app.Run())</param>
    /// <param name="applicationName">Nome da aplicação para logs</param>
    public static void RunWithSerilog(Action action, string applicationName)
    {
        try
        {
            Log.Information("Iniciando {ApplicationName}", applicationName);
            action();
            Log.Information("{ApplicationName} encerrado com sucesso", applicationName);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{ApplicationName} terminou inesperadamente", applicationName);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Executa uma aplicação assíncrona com tratamento de exceções e logging do Serilog.
    /// </summary>
    /// <param name="action">Ação assíncrona a ser executada</param>
    /// <param name="applicationName">Nome da aplicação para logs</param>
    public static async Task RunWithSerilogAsync(Func<Task> action, string applicationName)
    {
        try
        {
            Log.Information("Iniciando {ApplicationName}", applicationName);
            await action();
            Log.Information("{ApplicationName} encerrado com sucesso", applicationName);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{ApplicationName} terminou inesperadamente", applicationName);
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
