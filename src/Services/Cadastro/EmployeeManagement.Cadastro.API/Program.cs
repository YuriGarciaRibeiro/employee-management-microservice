using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Cadastro.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando Cadastro.API");

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
// Register MVC controllers required by MapControllers()
builder.Services.AddControllers();

// Swagger with JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management - Cadastro API",
        Version = "v1",
        Description = "API for employee management with JWT authentication"
    });

    // Definição do esquema de segurança: HTTP Bearer (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticação JWT via header Authorization. Informe apenas o token (o prefixo 'Bearer' é adicionado automaticamente pelo Swagger UI).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Requisito global de segurança: aplica o esquema Bearer a todas as operações
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


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee.CreateEmployeeCommand).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

// AutoMapper
builder.Services.AddAutoMapper(typeof(EmployeeMappingProfile));

// FluentValidation: register validators for DI and enable ASP.NET Core automatic validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapEmployeesEndpoints();
app.MapAuthEndpoints();

    app.Run();

    Log.Information("Cadastro.API encerrado com sucesso");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Cadastro.API terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}