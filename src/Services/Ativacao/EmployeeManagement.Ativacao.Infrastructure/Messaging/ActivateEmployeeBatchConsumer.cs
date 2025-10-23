namespace EmployeeManagement.Ativacao.Infrastructure.Messaging;

using Npgsql;

public class ActivateEmployeeBatchConsumer : IConsumer<ActivateEmployeeBatchEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ActivateEmployeeBatchConsumer> _logger;

    public ActivateEmployeeBatchConsumer(
        IConfiguration configuration,
        IPublishEndpoint publishEndpoint,
        ILogger<ActivateEmployeeBatchConsumer> logger)
    {
        _configuration = configuration;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ActivateEmployeeBatchEvent> context)
    {
        var batch = context.Message;

        _logger.LogInformation(
            "Processando lote {BatchNumber}/{TotalBatches} com {Count} funcionários para ativação",
            batch.BatchNumber, batch.TotalBatches, batch.Employees.Count);

        try
        {
            var connectionString = _configuration.GetConnectionString("CadastroConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Atualizar status de todos os funcionários do lote para 'Ativo'
                var employeeIds = batch.Employees.Select(e => e.EmployeeId).ToArray();

                var updateSql = @"
                    UPDATE cadastro.""Employees""
                    SET ""Status"" = 'Ativo', ""UpdatedAt"" = @UpdatedAt
                    WHERE ""Id"" = ANY(@EmployeeIds)";

                using var updateCommand = new NpgsqlCommand(updateSql, connection, transaction);
                updateCommand.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                updateCommand.Parameters.AddWithValue("@EmployeeIds", employeeIds);

                var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                _logger.LogInformation(
                    "Lote {BatchNumber}/{TotalBatches}: {RowsAffected} funcionários ativados no banco de dados",
                    batch.BatchNumber, batch.TotalBatches, rowsAffected);

                await transaction.CommitAsync();

                // Publicar eventos de ativação para cada funcionário (para o serviço de Notificações)
                foreach (var employee in batch.Employees)
                {
                    var activatedEvent = new EmployeeActivatedEvent
                    {
                        EmployeeId = employee.EmployeeId,
                        EmployeeName = employee.Name,
                        Department = employee.Department,
                        ActivatedAt = DateTime.Now
                    };

                    await _publishEndpoint.Publish(activatedEvent);
                }

                _logger.LogInformation(
                    "Lote {BatchNumber}/{TotalBatches}: Processamento concluído com sucesso. Eventos publicados para {Count} funcionários",
                    batch.BatchNumber, batch.TotalBatches, batch.Employees.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,
                    "Erro ao processar lote {BatchNumber}/{TotalBatches}. Rollback realizado.",
                    batch.BatchNumber, batch.TotalBatches);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao consumir lote {BatchNumber}/{TotalBatches}",
                batch.BatchNumber, batch.TotalBatches);
            throw; // Vai fazer retry automático do MassTransit
        }
    }
}
