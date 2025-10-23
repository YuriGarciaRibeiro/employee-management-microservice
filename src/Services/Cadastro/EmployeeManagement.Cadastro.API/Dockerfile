# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["EmployeeManagement.sln", "./"]

# Copy all project files
COPY ["src/Services/Cadastro/EmployeeManagement.Cadastro.API/EmployeeManagement.Cadastro.API.csproj", "src/Services/Cadastro/EmployeeManagement.Cadastro.API/"]
COPY ["src/Services/Cadastro/EmployeeManagement.Cadastro.Application/EmployeeManagement.Cadastro.Application.csproj", "src/Services/Cadastro/EmployeeManagement.Cadastro.Application/"]
COPY ["src/Services/Cadastro/EmployeeManagement.Cadastro.Domain/EmployeeManagement.Cadastro.Domain.csproj", "src/Services/Cadastro/EmployeeManagement.Cadastro.Domain/"]
COPY ["src/Services/Cadastro/EmployeeManagement.Cadastro.Infrastructure/EmployeeManagement.Cadastro.Infrastructure.csproj", "src/Services/Cadastro/EmployeeManagement.Cadastro.Infrastructure/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/EmployeeManagement.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Contracts/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/EmployeeManagement.BuildingBlocks.Core.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Core/"]
COPY ["src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/EmployeeManagement.BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/EmployeeManagement.BuildingBlocks.Messaging/"]

# Restore dependencies
RUN dotnet restore "src/Services/Cadastro/EmployeeManagement.Cadastro.API/EmployeeManagement.Cadastro.API.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/Services/Cadastro/EmployeeManagement.Cadastro.API"
RUN dotnet build "EmployeeManagement.Cadastro.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "EmployeeManagement.Cadastro.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
ENTRYPOINT ["dotnet", "EmployeeManagement.Cadastro.API.dll"]
