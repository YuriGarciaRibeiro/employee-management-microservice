using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.BuildingBlocks.Core.Extensions;

/// <summary>
/// Extensões para configurar CORS de forma centralizada.
/// </summary>
public static class CorsExtensions
{
    private const string DefaultPolicyName = "DefaultCorsPolicy";

    /// <summary>
    /// Adiciona política de CORS configurável baseada em origens permitidas.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="allowedOrigins">Origens permitidas (se vazio, permite qualquer origem - use apenas em desenvolvimento)</param>
    /// <param name="policyName">Nome da política</param>
    public static IServiceCollection AddConfigurableCors(
        this IServiceCollection services,
        string[]? allowedOrigins = null,
        string policyName = DefaultPolicyName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                if (allowedOrigins == null || allowedOrigins.Length == 0)
                {
                    // AVISO: Permitir qualquer origem é adequado apenas para desenvolvimento
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adiciona política de CORS para desenvolvimento (permite qualquer origem).
    /// AVISO: Use apenas em ambiente de desenvolvimento!
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="policyName">Nome da política</param>
    public static IServiceCollection AddDevelopmentCors(
        this IServiceCollection services,
        string policyName = DefaultPolicyName)
    {
        return services.AddConfigurableCors(null, policyName);
    }

    /// <summary>
    /// Adiciona política de CORS para produção com origens específicas.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="allowedOrigins">Origens permitidas</param>
    /// <param name="policyName">Nome da política</param>
    public static IServiceCollection AddProductionCors(
        this IServiceCollection services,
        string[] allowedOrigins,
        string policyName = DefaultPolicyName)
    {
        if (allowedOrigins == null || allowedOrigins.Length == 0)
        {
            throw new ArgumentException("Origens permitidas devem ser especificadas para produção", nameof(allowedOrigins));
        }

        return services.AddConfigurableCors(allowedOrigins, policyName);
    }
}
