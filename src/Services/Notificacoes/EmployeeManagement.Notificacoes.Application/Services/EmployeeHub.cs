using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Notificacoes.Application.Services;

public class EmployeeHub : Hub<IEmployeeHubClient>
{
    private readonly ILogger<EmployeeHub> _logger;

    public EmployeeHub(ILogger<EmployeeHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("Cliente conectado ao Hub. ConnectionId: {ConnectionId}", connectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("Cliente desconectado do Hub. ConnectionId: {ConnectionId}", connectionId);

        if (exception != null)
        {
            _logger.LogError(exception, "Erro ao desconectar cliente {ConnectionId}", connectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinDepartmentGroup(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
        {
            _logger.LogWarning("Tentativa de entrar em grupo com nome vazio. ConnectionId: {ConnectionId}", Context.ConnectionId);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, department);
        _logger.LogInformation("Cliente {ConnectionId} entrou no grupo {Department}", Context.ConnectionId, department);
    }


    public async Task LeaveDepartmentGroup(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
        {
            _logger.LogWarning("Tentativa de sair de grupo com nome vazio. ConnectionId: {ConnectionId}", Context.ConnectionId);
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, department);
        _logger.LogInformation("Cliente {ConnectionId} saiu do grupo {Department}", Context.ConnectionId, department);
    }
}
