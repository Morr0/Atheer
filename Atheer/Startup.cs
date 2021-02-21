using System;
using System.Reflection;
using Atheer.BackgroundServices;
using Atheer.Repositories;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Atheer.Services.UserSessionsService;
using Atheer.Utilities.Config.Models;
using Atheer.Utilities.Markdown;
using AutoMapper;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            services.Configure<Site>(Configuration.GetSection("Site"));

            services.AddControllersWithViews();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<ArticleFactory>();
            services.AddSingleton<UserFactory>();
            services.AddSingleton<TagFactory>();
            
            services.AddSingleton<MarkdownPipeline>(
                provider => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Use<CodeStyleExtension>().Build());

            // Repositories
            services.AddDbContext<Data>(opts =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("MainPostgres"));
            });

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

                    opts.SlidingExpiration = true;
                    var cookieTtl = TimeSpan.FromHours(4);
                    opts.ExpireTimeSpan = cookieTtl;
                    opts.Cookie.MaxAge = cookieTtl;
                    
                    opts.Validate();
                });

            // Background services
            services.AddHostedService<ReloadConfigBackgroundService>();
            services.AddHostedService<ScheduledArticlesReleaserBackgroundService>();
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
