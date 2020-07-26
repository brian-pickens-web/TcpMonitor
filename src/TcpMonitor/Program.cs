using Microsoft.Extensions.Caching.Memory;
using SimpleInjector;
using TcpMonitor.Views;
using Terminal.Gui;
using TcpMonitor.Extensions;
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
            Application.Run(_serviceProvider.GetService<App>());
        }

        private static Container ConfigureServices()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;

            container.Register<App>();
            container.Register<MainView>();
            container.Register<TcpPerformanceGrid>();
            container.Register<TcpConnectionsGrid>();

            container.Register<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
            container.Register<IProcessService, ProcessService>();
            container.Register<ITcpConnectionService, TcpConnectionService>();

            // Use the WinRM service if enabled for superior performance else use COM interop implementation
            var isWinRmEnabled = TcpPerformanceWinRMService.IsWindowsRemoteManagementEnabled();
            container.RegisterConditional<ITcpPerformanceService, TcpPerformanceWinRMService>(context => isWinRmEnabled);
            container.RegisterConditional<ITcpPerformanceService, TcpPerformanceComService>(context => !isWinRmEnabled);

            return container;
        }
    }
}
