using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Notificacoes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "Notificacoes API",
            timestamp = DateTime.UtcNow,
            signalRHub = "/employeeHub"
        });
    }

    /// <summary>
    /// Get SignalR Hub information
    /// </summary>
    [HttpGet("hub-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHubInfo()
    {
        return Ok(new
        {
            hubUrl = "/employeeHub",
            description = "SignalR Hub for real-time employee notifications",
            events = new[]
            {
                new { name = "EmployeeCreated", description = "Fired when a new employee is created" },
                new { name = "EmployeeActivated", description = "Fired when an employee is activated" },
                new { name = "EmployeeStartDateUpdated", description = "Fired when employee start date is updated" }
            },
            connectionExample = new
            {
                javascript = "const connection = new signalR.HubConnectionBuilder().withUrl('http://localhost:5002/employeeHub').build();",
                csharp = "var connection = new HubConnectionBuilder().WithUrl('http://localhost:5002/employeeHub').Build();"
            }
        });
    }
}
