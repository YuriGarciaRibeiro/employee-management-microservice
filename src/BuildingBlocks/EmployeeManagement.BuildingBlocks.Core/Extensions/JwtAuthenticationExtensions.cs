using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.BuildingBlocks.Core.Extensions;

/// <summary>
/// Extensões para configurar autenticação JWT de forma centralizada.
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Adiciona autenticação JWT com configurações do appsettings.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="jwtSettingsSection">Nome da seção de configuração (padrão: JwtSettings)</param>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string jwtSettingsSection = "JwtSettings")
    {
        var jwtSettings = configuration.GetSection(jwtSettingsSection);

        var secret = jwtSettings["Secret"]
            ?? throw new InvalidOperationException($"{jwtSettingsSection}:Secret não está configurado");

        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException($"{jwtSettingsSection}:Issuer não está configurado");

        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException($"{jwtSettingsSection}:Audience não está configurado");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Adiciona autenticação JWT com parâmetros customizados.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="secret">Chave secreta</param>
    /// <param name="issuer">Emissor do token</param>
    /// <param name="audience">Audiência do token</param>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        string secret,
        string issuer,
        string audience)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret não pode ser vazio", nameof(secret));

        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Issuer não pode ser vazio", nameof(issuer));

        if (string.IsNullOrWhiteSpace(audience))
            throw new ArgumentException("Audience não pode ser vazio", nameof(audience));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
