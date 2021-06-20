using System;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;
using Amazon.S3;
using Atheer.BackgroundServices;
using Atheer.Middlewares;
using Atheer.Repositories;
using Atheer.Services.ArticlesService;
using Atheer.Services.ArticlesService.Models;
using Atheer.Services.FileService;
using Atheer.Services.NavItemsService;
using Atheer.Services.OAuthService;
using Atheer.Services.QueueService;
using Atheer.Services.RecaptchaService;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using Atheer.Services.Utilities.TimeService;
using Atheer.Utilities;
using Atheer.Utilities.Config.Models;
using Atheer.Utilities.ETLs;
using Atheer.Utilities.Logging;
using AutoMapper;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;

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
            services.Configure<S3>(Configuration.GetSection("S3"));
            services.Configure<SiteAnalytics>(Configuration.GetSection("SiteAnalytics"));
            services.Configure<Recaptcha>(Configuration.GetSection("Recaptcha"));
            services.Configure<GithubOAuth>(Configuration.GetSection("GithubOAuth"));
            services.Configure<SQS>(Configuration.GetSection("SQS"));

            services.AddControllersWithViews(opts =>
            {
                opts.ReturnHttpNotAcceptable = true;
            });
            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<ArticleFactory>();
            services.AddSingleton<UserFactory>();
            services.AddSingleton<TagFactory>();
            services.AddSingleton<ITimeService, TimeService>();
            
            services.AddSingleton<MarkdownPipeline>(
                provider => Singletons.MarkdownPipeline);

            // Repositories
            services.AddDbContext<Data>(opts =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("MainPostgres"), dbOpts =>
                {
                    // TODO does not support user-initiated transactions. Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' to execute all the operations in the transaction as a retriable unit.
                    // dbOpts.EnableRetryOnFailure(3);
                });
            });
            services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(Configuration.GetConnectionString("MongoDB")));
            services.AddTransient<IAmazonS3, AmazonS3Client>();

            // Services
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IRecaptchaService, RecaptchaService>();
            services.AddTransient<ITagService, TagService>();
            services.AddScoped<IOAuthService, OAuthService>();
            services.AddSingleton<INavItemsService, NavItemService>();
            services.AddTransient<IQueueService, QueueService>();
            

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = "/Login";
                    opts.LogoutPath = "/Logout";

                    // Custom denied/forbidden redirection
                    opts.Events.OnRedirectToAccessDenied = (redirectContext) =>
                    {
                        string urlEncodedOriginalPath = HttpUtility.UrlEncode(redirectContext.Request.Path.ToString());
                        redirectContext.Response.Redirect($"/NotFound?path={urlEncodedOriginalPath}");

                        return Task.CompletedTask;
                    };
                    
                    opts.Cookie.HttpOnly = true;
                    opts.Cookie.IsEssential = true;

                    opts.SlidingExpiration = true;
                    var cookieTtl = TimeSpan.FromHours(4);
                    opts.ExpireTimeSpan = cookieTtl;
                    opts.Cookie.MaxAge = cookieTtl;
                    
                    opts.Validate();
                });

            services.AddSingleton<Channel<ArticleNarrationRequest>>(x => Channel.CreateUnbounded<ArticleNarrationRequest>());
            // Background services
            services.AddHostedService<ArticleNarrationRequesterBackgroundService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            IServiceScopeFactory serviceScopeFactory, Data postgresqlClient, IMongoClient mongoClient, ILoggerFactory loggerFactory)
        {
            Console.WriteLine(env.EnvironmentName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/Error");
            
            UpdateDatabase(serviceScopeFactory);
            
            // app.UseHttpsRedirection();

            app.UseStatusCodePagesWithReExecute("/HandleCode", "?code={0}");
            
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=86400";
                }
            });
            
            app.UseMiddleware<UrlRewritingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllers();
            });

            if (!string.IsNullOrEmpty(Configuration.GetConnectionString("MainPostgres")))
            {
                var logger = loggerFactory.CreateLogger(LoggingConstants.PostgresqlToMongoDBMigration);
                
                logger.LogCritical("Begun postgresql to mongodb operation");
                ETLPostgresqlToMongoDB.MigrateToMongo(postgresqlClient, mongoClient, logger);
                logger.LogCritical("Finished migrating to MongoDB");
            }
        }

        private void UpdateDatabase(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<Data>();
                
                context.Database.Migrate();
            }
        }
    }
}
