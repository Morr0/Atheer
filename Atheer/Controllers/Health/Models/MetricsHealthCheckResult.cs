namespace Atheer.Controllers.Health.Models
{
    public class MetricsHealthCheckResult : BasicHealthCheckResult
    {
        public bool CanReachDatabase { get; set; }
        public int CurrentDatabaseConnections { get; set; }
    }
}