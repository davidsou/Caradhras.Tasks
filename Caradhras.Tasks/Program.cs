using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caradhras.Tasks.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Caradhras.Tasks
{

    public class Program
    {

        static AppSettings appSettings = new AppSettings();
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            string path = "";

            if (isService)
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                path = Environment.CurrentDirectory + "\\";
            }

            var logger = NLogBuilder.ConfigureNLog($"{path}nlog.config").GetCurrentClassLogger();

            var config = new ConfigurationBuilder()
               .AddJsonFile($"{path}appsettings.json", optional: false)
               .Build();

            config.GetSection("AppSettings").Bind(appSettings);

            logger.Info(appSettings);

            IWebHost host;

            try
            {
                if (isService)
                {
                    Directory.SetCurrentDirectory(path);
                    host = BuildWebHost(args.Where(arg => arg != "--console").ToArray());
                    host.RunAsCustomService(); ;
                }
                else
                {
                    host = BuildWebHost(args);
                    host.Run();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
                 WebHost.CreateDefaultBuilder(args)
                        .UseStartup<Startup>()
                        .CaptureStartupErrors(true)
                           .ConfigureLogging((hostingContext, logging) =>
                           {
                               logging.ClearProviders();
                               logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                               logging.AddDebug();
                               logging.AddEventSourceLogger();
                           })
                        .UseNLog()  // NLog: setup NLog for Dependency injection
                        .UseUrls($"http://*:{appSettings.Porta}")
                        .Build();
    }
}
