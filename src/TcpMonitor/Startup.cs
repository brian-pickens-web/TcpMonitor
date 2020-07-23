using System;
using SimpleInjector;
using TcpMonitor.Views;

namespace TcpMonitor
{
    public class Startup
    {
        internal static IServiceProvider ConfigureServices()
        {
            var container = new Container();

            container.Register<App>();
            container.Register<MainView>();

            return container;
        }
    }
}
