# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["EmployeeManagement.sln", "./"]

# Copy all project files
COPY ["src/Services/Notificacoes/EmployeeManagement.Notificacoes.API/EmployeeManagement.Notificacoes.API.csproj", "src/Services/Notificacoes/EmployeeManagement.Notificacoes.API/"]
COPY ["src/Services/Notificacoes/EmployeeManagement.Notificacoes.Application/EmployeeManagement.Notificacoes.Application.csproj", "src/Services/Notificacoes/EmployeeManagement.Notificacoes.Application/"]
COPY ["src/Services/Notificacoes/EmployeeManagement.Notificacoes.Domain/EmployeeManagement.Notificacoes.Domain.csproj", "src/Services/Notificacoes/EmployeeManagement.Notificacoes.Domain/"]
COPY ["src/Services/Notificacoes/EmployeeManagement.Notificacoes.Infrastructure/EmployeeManagement.Notificacoes.Infrastructure.csproj", "src/Services/Notificacoes/EmployeeManagement.Notificacoes.Infrastructure/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/EmployeeManagement.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/EmployeeManagement.BuildingBlocks.Core.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/EmployeeManagement.BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/"]

# Restore dependencies
RUN dotnet restore "src/Services/Notificacoes/EmployeeManagement.Notificacoes.API/EmployeeManagement.Notificacoes.API.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/Services/Notificacoes/EmployeeManagement.Notificacoes.API"
RUN dotnet build "EmployeeManagement.Notificacoes.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "EmployeeManagement.Notificacoes.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

# Expose port
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "EmployeeManagement.Notificacoes.API.dll"]
