using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using HailowApiGateway.Database;
using HailowApiGateway.Config;

namespace HailowApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IBus _bus;
    private readonly AppConfig _config;

    public HealthController(AppDbContext dbContext, IBus bus, AppConfig config)
    {
        _dbContext = dbContext;
        _bus = bus;
        _config = config;
    }

    [HttpGet("check")]
    public async Task<IActionResult> Check()
    {
        var result = new
        {
            Database = await _dbContext.Database.CanConnectAsync(),
            RabbitMQ = await CheckRabbitMq(),
            Config = new
            {
                Postgres = _config.PostgresHost,
                RabbitMQ = _config.RabbitMqHost
            }
        };

        return Ok(result);
    }
    
    private async Task<bool> CheckRabbitMq()
    {
        try
        {
            await _bus.Publish(new { Test = "HealthCheck" });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
