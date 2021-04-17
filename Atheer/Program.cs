using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Atheer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(opts => opts.AddServerHeader = false)
                        .UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config)  =>
                {
                    var env = hostingContext.HostingEnvironment.EnvironmentName.ToLower();
                    if (env == "production")
                    {
                        config.AddSystemsManager("/Atheer");
                    }
                    
                });
    }
}
