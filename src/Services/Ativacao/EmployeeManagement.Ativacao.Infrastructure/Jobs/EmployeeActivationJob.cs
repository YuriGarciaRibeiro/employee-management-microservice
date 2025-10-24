namespace EmployeeManagement.Ativacao.Infrastructure.Jobs;

using EmployeeManagement.BuildingBlocks.Contracts.Events;
using Npgsql;

public class EmployeeActivationJob
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmployeeActivationJob> _logger;
    private const int BATCH_SIZE = 100;
    private const int LARGE_VOLUME_THRESHOLD = 1000;

    // DTO interno para evitar dependência do projeto Cadastro
    private class PendingEmployee
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
    }

    public EmployeeActivationJob(
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration,
        ILogger<EmployeeActivationJob> logger)
    {
        _publishEndpoint = publishEndpoint;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Iniciando job de ativação automática de funcionários às {Time}", DateTime.UtcNow);

        try
        {
            var today = DateTime.UtcNow.Date;

            var employees = await GetPendingEmployeesAsync(today);

            if (!employees.Any())
            {
                _logger.LogInformation("Nenhum funcionário encontrado para ativação hoje ({Date})", today);
                return;
            }

            _logger.LogInformation("Encontrados {Count} funcionários para ativação em {Date}",
                employees.Count, today);

            if (employees.Count <= LARGE_VOLUME_THRESHOLD)
            {
                _logger.LogInformation("Volume pequeno ({Count} <= {Threshold}). Processando em lote único.",
                    employees.Count, LARGE_VOLUME_THRESHOLD);

                await ActivateBatchAsync(employees, 1, 1);
            }
            else
            {
                _logger.LogInformation("Volume grande ({Count} > {Threshold}). Processando em lotes de {BatchSize}.",
                    employees.Count, LARGE_VOLUME_THRESHOLD, BATCH_SIZE);

                var batches = employees
                    .Select((employee, index) => new { employee, index })
                    .GroupBy(x => x.index / BATCH_SIZE)
                    .Select(g => g.Select(x => x.employee).ToList())
                    .ToList();

                var totalBatches = batches.Count;

                for (int i = 0; i < batches.Count; i++)
                {
                    var batchNumber = i + 1;
                    await ActivateBatchAsync(batches[i], batchNumber, totalBatches);

                    _logger.LogInformation("Lote {BatchNumber}/{TotalBatches} processado com {Count} funcionários",
                        batchNumber, totalBatches, batches[i].Count);
                }
            }

            _logger.LogInformation("Job de ativação concluído com sucesso. Total processado: {Count} funcionários",
                employees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de ativação de funcionários");
            throw;
        }
    }

    private async Task<List<PendingEmployee>> GetPendingEmployeesAsync(DateTime targetDate)
    {
        var connectionString = _configuration.GetConnectionString("CadastroConnection");

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""Name"", ""Department"", ""StartDate""
            FROM cadastro.""Employees""
            WHERE DATE(""StartDate"") = @TargetDate
              AND ""Status"" = 'Inativo'";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TargetDate", targetDate);

        var employees = new List<PendingEmployee>();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(new PendingEmployee
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Department = reader.GetString(2),
                StartDate = reader.GetDateTime(3)
            });
        }

        return employees;
    }

    private async Task ActivateBatchAsync(List<PendingEmployee> employees, int batchNumber, int totalBatches)
    {
        var batchEvent = new ActivateEmployeeBatchEvent
        {
            Employees = employees.Select(e => new EmployeeBatchDto
            {
                EmployeeId = e.Id,
                Name = e.Name,
                Department = e.Department,
                StartDate = e.StartDate
            }).ToList(),
            BatchNumber = batchNumber,
            TotalBatches = totalBatches,
            ScheduledDate = DateTime.UtcNow.Date
        };

        await _publishEndpoint.Publish(batchEvent);

        _logger.LogInformation(
            "Lote {BatchNumber}/{TotalBatches} publicado na fila com {Count} funcionários",
            batchNumber, totalBatches, employees.Count);
    }
}
