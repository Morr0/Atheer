using System;
using System.Reflection;
using System.Security.Claims;
using Atheer.Repositories.Blog;
using Atheer.Services.BlogService;
using Atheer.Utilities.Config.Models;
using AutoMapper;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atheer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DynamoDbTables>(Configuration.GetSection("DynamoDbTables"));
            services.Configure<Site>(Configuration.GetSection("Site"));
            
            services.AddControllersWithViews();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<BlogPostFactory>();
            services.AddSingleton<MarkdownPipeline>(
                provider => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build());

            // Repositories
            services.AddSingleton<BlogPostRepository>();

            // Services
            services.AddTransient<IBlogPostService, BlogPostService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = "/login";
                    opts.LogoutPath = "/logout";
                    opts.AccessDeniedPath = "/denied";

                    opts.ExpireTimeSpan = DateTimeOffset.UtcNow.AddHours(4).Offset;
                });

            // services.AddAuthorization(opts =>
            // {
            //     opts.AddPolicy("User", builder =>
            //     {
            //         builder.RequireClaim(ClaimTypes.Name);
            //         builder.RequireAuthenticatedUser();
            //     });
            // });
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
