using Alidu.Core.Domain.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.ServiceHost
{
    public static class DefaultProgram
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


        public static async Task<int> MainProgram<TStartup>(string appName, Action<IUnitOfWork, IServiceProvider> migrateSeeder, string[] args) where TStartup : class
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(appName, configuration);
            try
            {
                Log.Information("Configuring web host ({AppName})...", appName);
                var host = BuildWebHost<TStartup>(configuration, args);

                Log.Information("Applying migrations ({AppName})...", appName);
                host.MigrateDbContext(migrateSeeder);

                Log.Information("Starting web host ({AppName})...", appName);
                await host.RunWithTasksAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({AppName})!", appName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static IWebHost BuildWebHost<TStartup>(IConfiguration configuration, string[] args) where TStartup : class
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration((builderContext, builder) =>
                {
                    BuildConfiguration(builder);
                })
                .UseStartup<TStartup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .ConfigureLogging(logging => logging.AddSerilog())
                .ConfigureKestrel(serverOptions =>
                {

                })
                .Build();
        }
        public static Serilog.ILogger CreateSerilogLogger(string appName, IConfiguration configuration)
        {
            var filePath = configuration["Serilog:FilePath"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("AppName", appName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(filePath
                    , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    , rollingInterval: RollingInterval.Day
                    , rollOnFileSizeLimit: true
                    , shared: true
                    //, buffered: true
                    )
                .Enrich.WithProperty("Environment", EnvironmentName)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IConfiguration GetConfiguration()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory);

            BuildConfiguration(builder);

            return builder.Build();
        }

        public static void BuildConfiguration(IConfigurationBuilder builder)
        {
            foreach (var jsonFilename in Directory.EnumerateFiles("config", "*.json", SearchOption.AllDirectories))
                builder.AddJsonFile(jsonFilename);

            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.local.json", optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables();

        }
    }
}
