# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["EmployeeManagement.sln", "./"]

# Copy all project files
COPY ["src/Services/Ativacao/EmployeeManagement.Ativacao.Worker/EmployeeManagement.Ativacao.Worker.csproj", "src/Services/Ativacao/EmployeeManagement.Ativacao.Worker/"]
COPY ["src/Services/Ativacao/EmployeeManagement.Ativacao.Application/EmployeeManagement.Ativacao.Application.csproj", "src/Services/Ativacao/EmployeeManagement.Ativacao.Application/"]
COPY ["src/Services/Ativacao/EmployeeManagement.Ativacao.Domain/EmployeeManagement.Ativacao.Domain.csproj", "src/Services/Ativacao/EmployeeManagement.Ativacao.Domain/"]
COPY ["src/Services/Ativacao/EmployeeManagement.Ativacao.Infrastructure/EmployeeManagement.Ativacao.Infrastructure.csproj", "src/Services/Ativacao/EmployeeManagement.Ativacao.Infrastructure/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/EmployeeManagement.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/EmployeeManagement.BuildingBlocks.Core.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/EmployeeManagement.BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/"]

# Restore dependencies
RUN dotnet restore "src/Services/Ativacao/EmployeeManagement.Ativacao.Worker/EmployeeManagement.Ativacao.Worker.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/Services/Ativacao/EmployeeManagement.Ativacao.Worker"
RUN dotnet build "EmployeeManagement.Ativacao.Worker.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "EmployeeManagement.Ativacao.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 appuser && \
    adduser --system --uid 1001 --ingroup appuser appuser

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Health check (worker process check)
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD pgrep -f "dotnet.*EmployeeManagement.Ativacao.Worker" > /dev/null || exit 1

# Entry point
ENTRYPOINT ["dotnet", "EmployeeManagement.Ativacao.Worker.dll"]
