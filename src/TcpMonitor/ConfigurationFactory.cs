using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using SimpleInjector;
using TcpMonitor.Services;
using TcpMonitor.Views;
using TcpMonitor.Views.Data;

namespace TcpMonitor
{
    public class ConfigurationFactory
    {
        private static readonly IHostEnvironment Environment = new HostingEnvironment
        {
            EnvironmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            ApplicationName = AppDomain.CurrentDomain.FriendlyName,
            ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
            ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
        };

        public static IConfiguration CreateConfiguration()
        {
            var config = new ConfigurationBuilder();
            var configured = config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return configured.Build();
        }

        public static Container CreateContainer()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;

            RegisterViews(container);
            RegisterServices(container);
            RegisterLogging(container);

            return container;
        }

        private static void RegisterViews(Container container)
        {
            container.Register<App>();
            container.Register<MainView>();
            container.Register<MenuBarView>();
            container.Register<SideMenuView>();
            container.Register<DashboardView>();
            container.Register<TcpConnectionsView>();
            container.Register<TcpPerformanceGrid>();
            container.Register<TcpConnectionsGrid>();
            container.Register<TcpSettingsGrid>();
        }

        private static void RegisterServices(Container container)
        {
            container.Register<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
            container.Register<IProcessService, ProcessService>();
            container.Register<ITcpConnectionService, TcpConnectionService>();

            // Use the WinRM service if enabled for superior performance else use COM interop implementation
            var isWinRmEnabled = TcpWinRMService.IsWindowsRemoteManagementEnabled();
            container.RegisterConditional<ITcpPerformanceService, TcpWinRMService>(context => isWinRmEnabled);
            container.RegisterConditional<ITcpPerformanceService, TcpPerformanceComService>(context => !isWinRmEnabled);
            container.RegisterConditional<ITcpSettingsService, TcpWinRMService>(context => isWinRmEnabled);
            container.RegisterConditional<ITcpSettingsService, TcpSettingsComService>(context => !isWinRmEnabled);
        }

        private static void RegisterLogging(Container container)
        {
            var serilogLogger = CreateLogger();
            container.Register<ILoggerFactory>(() => new SerilogLoggerFactory(serilogLogger));
            container.Register<Microsoft.Extensions.Logging.ILogger>(() =>
                container.GetInstance<ILoggerFactory>().AddSerilog(serilogLogger).CreateLogger("logger"));
        }

        private static Logger CreateLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

            if (Debugger.IsAttached)
            {
                loggerConfiguration.MinimumLevel.Verbose();
                loggerConfiguration.WriteTo.Debug();
            }
            else
            {
                loggerConfiguration.MinimumLevel.Error();
                loggerConfiguration.WriteTo.RollingFile("log.txt");
            }

            return loggerConfiguration.CreateLogger();
        }
    }
}
