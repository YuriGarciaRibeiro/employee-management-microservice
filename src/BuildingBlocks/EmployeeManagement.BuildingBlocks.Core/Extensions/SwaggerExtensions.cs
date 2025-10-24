using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EmployeeManagement.BuildingBlocks.Core.Extensions;

/// <summary>
/// Extensões para configurar Swagger/OpenAPI de forma centralizada.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adiciona Swagger com configuração básica.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="title">Título da API</param>
    /// <param name="version">Versão da API</param>
    /// <param name="description">Descrição da API</param>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services,
        string title,
        string version = "v1",
        string? description = null)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description
            });
        });

        return services;
    }

    /// <summary>
    /// Adiciona Swagger com suporte a autenticação JWT.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="title">Título da API</param>
    /// <param name="version">Versão da API</param>
    /// <param name="description">Descrição da API</param>
    public static IServiceCollection AddSwaggerWithJwt(
        this IServiceCollection services,
        string title,
        string version = "v1",
        string? description = null)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description
            });

            // Configuração de segurança JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autenticação JWT via header Authorization. Informe apenas o token (o prefixo 'Bearer' é adicionado automaticamente pelo Swagger UI).",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
