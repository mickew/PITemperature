using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PiTemperature.Meters;
using Microsoft.AspNet.SignalR;
using PiTemperature.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using PiTemperature.Repositories;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace PiTemperature
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure our application database context to be Entity Framework backed by SQL Server
            var path = PlatformServices.Default.Application.ApplicationBasePath;
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<ApplicationDbContext>(otions => 
                    //otions.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"]));
                    otions.UseSqlite("Filename=" + Path.Combine(path, "pitemperature.db")));

            // Specify the configuration of our Application database context
            services.Configure<ApplicationDbContextOptions>(options =>
            {
                options.DefaultUsername = Configuration["DefaultUser:Username"];
                options.DefaultPassword = Configuration["DefaultUSer:Password"];
            });

            // Configure ASP.NET Identity to use our Identity-based application context
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add framework services.
            services.AddMvc();
            services.AddScoped<ISensorRepository, SensorRepository>();
            services.AddSingleton<Temperature>();
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(options =>
                {
                    options.EnableAll();
                });
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseIISPlatformHandler();

            //app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseIdentity();

            app.UseMvc();
            app.UseSignalR();
            ApplicationDbContext.InitializeDatabaseAsync(app.ApplicationServices).Wait();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
