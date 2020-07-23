using System;
using SimpleInjector;
using SimpleInjector.Lifestyles;
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
        private static Container _serviceProvider;

        public static void Main()
        {
            _serviceProvider = ConfigureServices();
            using Scope scope = AsyncScopedLifestyle.BeginScope(_serviceProvider);
            Application.Init();
            Application.Run(scope.GetService<App>());
        }

        private static Container ConfigureServices()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;

            container.Register<App>();
            container.Register<MainView>();

            return container;
        }
    }
}
