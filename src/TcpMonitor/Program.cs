using Microsoft.Extensions.Caching.Memory;
using SimpleInjector;
using TcpMonitor.Views;
using Terminal.Gui;
using TcpMonitor.Services;

namespace TcpMonitor
{
    /// <summary>
    /// Uses Terminal.Gui at https://github.com/migueldeicaza/gui.cs
    /// </summary>
    public class Program
    {
        private static Container _serviceProvider;

        public static void Main()
        {
            _serviceProvider = ConfigureServices();
            Application.Init();
            Application.Run(_serviceProvider.GetInstance<App>());
        }

        private static Container ConfigureServices()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;

            container.Register<App>();
            container.Register<MainView>();
            container.Register<TcpPerformanceGrid>();
            container.Register<TcpConnectionsGrid>();
            container.Register<TcpSettingsGrid>();

            container.Register<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
            container.Register<IProcessService, ProcessService>();
            container.Register<ITcpConnectionService, TcpConnectionService>();

            // Use the WinRM service if enabled for superior performance else use COM interop implementation
            var isWinRmEnabled = TcpWinRMService.IsWindowsRemoteManagementEnabled();
            container.RegisterConditional<ITcpPerformanceService, TcpWinRMService>(context => isWinRmEnabled);
            container.RegisterConditional<ITcpPerformanceService, TcpPerformanceComService>(context => !isWinRmEnabled);
            container.RegisterConditional<ITcpSettingsService, TcpWinRMService>(context => isWinRmEnabled);
            container.RegisterConditional<ITcpSettingsService, TcpSettingsComService>(context => !isWinRmEnabled);

            return container;
        }
    }
}
