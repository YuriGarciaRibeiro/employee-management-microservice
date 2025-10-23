using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Cadastro.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        try
        {
            logger.LogInformation("Iniciando inicialização do banco de dados...");

            // Aplicar migrations pendentes
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Aplicando migrations pendentes...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso");
            }

            // Seed de Roles
            await SeedRolesAsync(roleManager, logger);

            // Seed de Usuários
            await SeedUsersAsync(userManager, logger);

            // Seed de Funcionários
            await SeedEmployeesAsync(context, logger);

            logger.LogInformation("Inicialização do banco de dados concluída com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inicializar banco de dados");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Verificando roles...");

        string[] roles = { "Admin", "User", "Manager" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                logger.LogInformation("Criando role: {RoleName}", roleName);
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        logger.LogInformation("Roles verificadas/criadas com sucesso");
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        logger.LogInformation("Verificando usuários padrão...");

        // Usuário Admin
        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            logger.LogInformation("Criando usuário admin...");
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Usuário admin criado: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Erro ao criar usuário admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Usuário Demo
        var demoEmail = "demo@example.com";
        if (await userManager.FindByEmailAsync(demoEmail) == null)
        {
            logger.LogInformation("Criando usuário demo...");
            var demoUser = new ApplicationUser
            {
                UserName = demoEmail,
                Email = demoEmail,
                EmailConfirmed = true,
                FullName = "Demo User",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(demoUser, "Demo@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoUser, "User");
                logger.LogInformation("Usuário demo criado: {Email}", demoEmail);
            }
            else
            {
                logger.LogError("Erro ao criar usuário demo: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Usuário Manager
        var managerEmail = "manager@example.com";
        if (await userManager.FindByEmailAsync(managerEmail) == null)
        {
            logger.LogInformation("Criando usuário manager...");
            var managerUser = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                FullName = "Manager User",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(managerUser, "Manager@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
                logger.LogInformation("Usuário manager criado: {Email}", managerEmail);
            }
            else
            {
                logger.LogError("Erro ao criar usuário manager: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        logger.LogInformation("Usuários padrão verificados/criados com sucesso");
    }

    private static async Task SeedEmployeesAsync(ApplicationDbContext context, ILogger logger)
    {
        logger.LogInformation("Verificando funcionários...");

        if (await context.Employees.AnyAsync())
        {
            logger.LogInformation("Funcionários já existem no banco. Pulando seed.");
            return;
        }

        logger.LogInformation("Criando funcionários de exemplo...");

        var departments = new[] { "TI", "RH", "Vendas", "Financeiro", "Marketing" };
        var employees = new List<Employee>();

        // 1. Funcionários ativos (já iniciados)
        for (int i = 1; i <= 10; i++)
        {
            employees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Funcionário Ativo {i}",
                Phone = $"+5511{90000000 + i}",
                Department = departments[i % departments.Length],
                StartDate = DateTime.UtcNow.AddDays(-30 - i),
                Status = EmployeeStatus.Ativo,
                CreatedAt = DateTime.UtcNow.AddDays(-35 - i)
            });
        }

        // 2. Funcionários inativos (início futuro próximo)
        for (int i = 1; i <= 15; i++)
        {
            employees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Funcionário Futuro {i}",
                Phone = $"+5511{91000000 + i}",
                Department = departments[i % departments.Length],
                StartDate = DateTime.UtcNow.AddDays(i),
                Status = EmployeeStatus.Inativo,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        // 3. Funcionários para teste de ativação em lote (mesma data de início)
        // 150 funcionários para testar processamento em lotes de 100
        var batchTestDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7).Date, DateTimeKind.Utc); // 7 dias no futuro
        for (int i = 1; i <= 150; i++)
        {
            employees.Add(new Employee
            {
                Id = Guid.NewGuid(),
                Name = $"Funcionário Lote {i}",
                Phone = $"+5511{92000000 + i}",
                Department = departments[i % departments.Length],
                StartDate = batchTestDate,
                Status = EmployeeStatus.Inativo,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        // 4. Alguns funcionários com datas variadas para testes de filtro
        employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Name = "João Silva",
            Phone = "+551199999001",
            Department = "TI",
            StartDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = EmployeeStatus.Ativo,
            CreatedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc)
        });

        employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Maria Santos",
            Phone = "+551199999002",
            Department = "RH",
            StartDate = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            Status = EmployeeStatus.Inativo,
            CreatedAt = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc)
        });

        employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Pedro Oliveira",
            Phone = "+551199999003",
            Department = "Vendas",
            StartDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            Status = EmployeeStatus.Inativo,
            CreatedAt = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc)
        });

        employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Ana Costa",
            Phone = "+551199999004",
            Department = "Financeiro",
            StartDate = DateTime.UtcNow.AddDays(-15),
            Status = EmployeeStatus.Ativo,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        });

        employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Carlos Ferreira",
            Phone = "+551199999005",
            Department = "Marketing",
            StartDate = DateTime.UtcNow.AddDays(30),
            Status = EmployeeStatus.Inativo,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        });

        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();

        logger.LogInformation("Criados {Count} funcionários de exemplo", employees.Count);
        logger.LogInformation("  - 10 ativos");
        logger.LogInformation("  - 15 inativos (início próximo)");
        logger.LogInformation("  - 150 para teste de lote (data: {Date})", batchTestDate.ToString("yyyy-MM-dd"));
        logger.LogInformation("  - 5 funcionários específicos para testes");
    }
}
