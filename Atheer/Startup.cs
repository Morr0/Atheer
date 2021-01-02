using System;
using System.Reflection;
using Atheer.Repositories.Blog;
using Atheer.Services.BlogService;
using Atheer.Utilities;
using AutoMapper;
using Markdig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atheer
{
    public class Startup
    {
        private AtheerConfig _config;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = new AtheerConfig(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<AtheerConfig>(_config);
            services.AddSingleton<MarkdownPipeline>(
                provider => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build());

            // Repositories
            services.AddTransient<BlogPostRepository>();

            // Services
            services.AddTransient<IBlogPostService, BlogPostService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("Development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                Console.WriteLine("Production");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // TODO Configure this
                // app.UseHsts();
            }
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
