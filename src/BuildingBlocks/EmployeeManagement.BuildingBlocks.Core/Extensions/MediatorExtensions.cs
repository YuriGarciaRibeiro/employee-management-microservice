using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.BuildingBlocks.Core.Extensions;

/// <summary>
/// Extensões para configurar MediatR de forma centralizada.
/// Centraliza a versão do MediatR.
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Adiciona MediatR registrando handlers de um assembly específico.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assembly">Assembly contendo os handlers</param>
    public static IServiceCollection AddMediatorFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        return services;
    }

    /// <summary>
    /// Adiciona MediatR registrando handlers de múltiplos assemblies.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies contendo os handlers</param>
    public static IServiceCollection AddMediatorFromAssemblies(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        return services;
    }
}
