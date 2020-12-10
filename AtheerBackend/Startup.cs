using System;
using AtheerBackend.Repositories.Blog;
using AtheerBackend.Repositories.Contact;
using AtheerBackend.Services.BlogService;
using AtheerBackend.Services.ContactService;
using AtheerBackend.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AtheerBackend
{
    public class Startup
    {
        private AtheerConfig _config;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            _config = new AtheerConfig(configuration);
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            services.AddSingleton<AtheerConfig>(_config);

            // Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // Repositories
            services.AddTransient<ContactRepository>();
            services.AddTransient<BlogPostRepository>();

            // Services
            services.AddTransient<IBlogPostService, BlogPostService>();
            services.AddTransient<IContactService, ContactService>();

            services.AddCors((opts) =>
            {
                opts.AddDefaultPolicy((policy) =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                Console.WriteLine("Development");
            }
            
            app.UseCors();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
