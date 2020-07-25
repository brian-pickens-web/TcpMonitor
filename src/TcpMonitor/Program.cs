using System;
using System.Diagnostics;
using SimpleInjector;
using SimpleInjector.Lifestyles;
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

            return container;
        }
    }
}
