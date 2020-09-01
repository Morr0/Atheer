using AtheerBlogWriterBackend.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace AtheerBlogWriterBackend
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IBlogEditorService, BlogEditorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // To allow to reread the request streams again
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
