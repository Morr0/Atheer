using System;
using System.Threading;
using System.Threading.Tasks;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Atheer.BackgroundServices
{
    public class ReloadConfigBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfigurationRoot _configurationRoot;

        public ReloadConfigBackgroundService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _configurationRoot = configuration as IConfigurationRoot;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                using var scope = _scopeFactory.CreateScope();
                var siteConfig = scope.ServiceProvider.GetService<IOptionsSnapshot<Site>>();

                int reloadDurationMs = siteConfig?.Value.ConfigReloadDuration == default
                    ? TimeSpan.FromHours(1).Milliseconds
                    : siteConfig.Value.ConfigReloadDuration * 1000;
                
                await Task.Delay(reloadDurationMs, stoppingToken).ConfigureAwait(false);
                _configurationRoot.Reload();
            }
        }
    }
}