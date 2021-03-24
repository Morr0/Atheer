using Atheer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Atheer.Controllers.Health.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Atheer.Controllers.Health
{
    [Route("/Health")]
    [ApiController]
    // TODO allow certain endpoints to only be accessed from within the same network
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult HealthyPublic()
        {
            return Ok();
        }

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

            return Ok(result);
        }
    }
}