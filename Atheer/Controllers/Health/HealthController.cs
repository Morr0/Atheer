using Atheer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Atheer.Controllers.Health.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Atheer.Controllers.Utilities.Filters;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Health
{
    [Route("/Health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult HealthyPublic()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Health check probe from IP: {Ip}", ipAddress);
            
            return Ok();
        }

        [RestrictToInternalNetwork]
        [HttpGet("internal/metrics")]
        public async Task<IActionResult> HealthyInternalComprehensive([FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            bool canConnectToDb;
            int currentDbConnections = 0;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Data>();

                canConnectToDb = await context.Database.CanConnectAsync().ConfigureAwait(false);
                if (canConnectToDb)
                {
                    currentDbConnections = await context.PgStatActivity
                        .CountAsync(x => x.State == "active").ConfigureAwait(false);
                }
            }

            var result = new MetricsHealthCheckResult
            {
                Healthy = true,
                CanReachDatabase = canConnectToDb,
                CurrentDatabaseConnections = currentDbConnections,
            };
            
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Internal health check probe");

            return Ok(result);
        }
    }
}