using System;
using AtheerBackend.Repositories.Contact;
using AtheerBackend.Services.BlogService;
using AtheerBackend.Services.ContactService;
using AtheerCore;
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
        private ConstantsLoader _constantsLoader;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            _constantsLoader = new ConstantsLoader(configuration);
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            services.AddSingleton<ConstantsLoader>(_constantsLoader);

            // Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // Repositories
            services.AddTransient<ContactRepository>();

            // Services
            services.AddTransient<IBlogRepository, BlogRepository>();
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
            }

            app.UseHttpsRedirection();

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
