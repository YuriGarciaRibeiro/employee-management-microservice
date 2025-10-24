using EmployeeManagement.Cadastro.Application.Behaviors;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Infrastructure.Data;
using EmployeeManagement.BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.AddSerilogLogging("Cadastro.API");

// Controllers
builder.Services.AddControllers();

// Swagger com autenticação JWT
builder.Services.AddSwaggerWithJwt(
    title: "Employee Management - Cadastro API",
    description: "API for employee management with JWT authentication");

// MediatR com Validation Pipeline Behavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee.CreateEmployeeCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Infrastructure (DbContext, Repositories, Identity, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// AutoMapper
builder.Services.AddAutoMapper(typeof(EmployeeMappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee.CreateEmployeeCommand).Assembly);

// Autenticação JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS - Permitir todas origens apenas em desenvolvimento
builder.Services.AddDevelopmentCors();

var app = builder.Build();

// Initialize database with migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await DbInitializer.InitializeAsync(context, userManager, roleManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao inicializar o banco de dados");
    }
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapEmployeesEndpoints();
app.MapAuthEndpoints();

// Executar aplicação com logging do Serilog
SerilogExtensions.RunWithSerilog(() => app.Run(), "Cadastro.API");