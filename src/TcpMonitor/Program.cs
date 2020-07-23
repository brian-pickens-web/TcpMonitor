using System;
using TcpMonitor.Views;
using Terminal.Gui;
using TcpMonitor.Extensions;

namespace TcpMonitor
{
    /// <summary>
    /// Uses Terminal.Gui at https://github.com/migueldeicaza/gui.cs
    /// </summary>
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static void Main()
        {
            _serviceProvider = Startup.ConfigureServices();
            Application.Init();
            Application.Run(_serviceProvider.GetService<App>());
        }
    }
}
