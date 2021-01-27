using System;
using System.Reflection;
using Atheer.BackgroundServices;
using Atheer.Repositories;
using Atheer.Repositories.Blog;
using Atheer.Services.BlogService;
using Atheer.Services.UserService;
using Atheer.Services.UserSessionsService;
using Atheer.Utilities.Config.Models;
using AutoMapper;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            services.AddSingleton<ArticleFactory>();
            services.AddSingleton<UserFactory>();
            
            services.AddSingleton<MarkdownPipeline>(
                provider => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build());

            // Repositories
            services.AddSingleton<ArticleRepository>();
            services.AddSingleton<UserRepository>();

            // Services
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserSessionsService, UserSessionsService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = "/Login";
                    opts.LogoutPath = "/Logout";
                    opts.AccessDeniedPath = "/Denied";

                    opts.Cookie.HttpOnly = true;
                    opts.Cookie.IsEssential = true;
                });

            // Background services
            services.AddHostedService<ReloadConfigBackgroundService>();
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
                app.UseExceptionHandler("/Error");
            }
            
            // app.UseHttpsRedirection();

            app.UseStatusCodePagesWithRedirects("/NotFound");
            
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
