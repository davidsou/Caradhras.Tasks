using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Caradhras.Tasks.Domain.Entities;
using Caradhras.Tasks.Repository;
using Caradhras.Tasks.Service.Interfaces;
using Caradhras.Tasks.Service.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Caradhras.Tasks
{
    public class Startup
    {
        private AppSettings _appSettings;
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; set; }
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILogger<Startup> logger)
        {
            var isService = !Debugger.IsAttached;

            string path = "";

            if (isService)
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                path = Environment.CurrentDirectory + "\\";
            }

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile($"{path}appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{path}appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
            _logger = logger;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;

            });

            var config = Configuration.GetSection("AppSettings").Get<AppSettings>();
            _appSettings = config;
            services.AddOptions();
            services.AddMvc();

            services.Configure<AppSettings>(Configuration.GetSection("appsettings"));
            services.AddScoped<ICaradhrasRepository, CaradhrasRepository>();
            services.AddScoped<ISFTPService, SFTPService>();
            services.AddScoped<ICashBackRepository, CashBackRepository>();
            services.AddScoped<IScheduledService, ScheduledService>();
            services.AddScoped<ITasksServices, TasksService>();

            services.AddScoped<ITaskRepository, TaskRepository>();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("BackOffice"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(15),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(15),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Microservice Tasks", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
          
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice Tasks V1");
            });

            //Fixar Cultura para pt-BR
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("pt-BR") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("pt-BR") },
                DefaultRequestCulture = new RequestCulture("pt-BR")
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            InitProcess();
        }

        private void InitProcess()
        {
            RecurringJob.AddOrUpdate<ScheduledService>(x => x.CheckReceiptsRequest(), _appSettings.CronScheduleRequestFile);
            RecurringJob.AddOrUpdate<ScheduledService>(x => x.GetReceipts(), _appSettings.CronScheduleDownloadFile);
        }
    }
}
